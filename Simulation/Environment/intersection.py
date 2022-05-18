from statistics import pstdev

from gym.vector.utils import spaces
import numpy as np


class Intersection:
    def __init__(self, sumo, id: str, delta_time: int, yellow_time: int, use_default_phases: bool = False, min_green_time: int = 5):
        self.sumo = sumo
        self.id = id
        self.delta_time = delta_time
        self.yellow_time = yellow_time
        self.use_default_phases = use_default_phases
        self.min_green_time = min_green_time

        self.green_phase = 0
        self.time_since_last_phase_change = 0
        self.next_action_time = 0

        self.green_phases = []
        self.yellow_dict = {}
        self.is_yellow = False
        self.last_reward = 0

        self.last_lanes_queue = None
        self.last_waiting_time = None

        self.vehicles = dict()
        self.vehicles_wait_time = []

        self.vehicle_size_min_gap = 5 + 2.5  # vehSize + minGap


    def setup(self):
        self.lanes = list(dict.fromkeys(self.sumo.trafficlight.getControlledLanes(self.id)))
        self.out_lanes = list(set([link[0][1] for link in self.sumo.trafficlight.getControlledLinks(self.id) if link]))
        self.lanes_lenght = {lane: self.sumo.lane.getLength(lane) for lane in self.lanes}

        self.build_phases()

        self.observations = spaces.Box(
            low=np.zeros(len(self.green_phases) + 1 + 2 * len(self.lanes), dtype=np.float32),
            high=np.ones(len(self.green_phases) + 1 + 2 * len(self.lanes), dtype=np.float32))

        self.actions = spaces.Discrete(len(self.green_phases))


    def build_phases(self):
        logics = self.sumo.trafficlight.getAllProgramLogics(self.id)

        if self.use_default_phases:
            self.green_phases = [i for i in range((len(logics[0].phases) // 2))]
            return

        self.green_phases = []
        self.yellow_dict = {}

        for phase in logics[0].phases:
            state = phase.state
            if 'y' not in state and state.count('r') + state.count('s') != len(state):
                self.green_phases.append(self.sumo.trafficlight.Phase(60, state))

        self.phases = self.green_phases.copy()

        for i, p1 in enumerate(self.green_phases):
            for j, p2 in enumerate(self.green_phases):
                if i == j: continue
                yellow_state = ''
                for s in range(len(p1.state)):
                    if (p1.state[s] == 'G' or p1.state[s] == 'g') and (p2.state[s] == 'r' or p2.state[s] == 's'):
                        yellow_state += 'y'
                    else:
                        yellow_state += p1.state[s]
                self.yellow_dict[(i,j)] = len(self.phases)
                self.phases.append(self.sumo.trafficlight.Phase(self.yellow_time, yellow_state))

        logic = logics[0]
        logic.type = 0
        logic.phases = self.phases

        self.sumo.trafficlight.setProgramLogic(self.id, logic)
        self.sumo.trafficlight.setRedYellowGreenState(self.id, self.phases[0].state)

    def compute_observation(self):
        phase_id = [1 if self.green_phase == i else 0 for i in range(len(self.green_phases))]
        observation = np.array(phase_id + self.get_lanes_density() + self.get_lanes_queue(), dtype=np.float32)
        return observation

    def compute_reward(self):
        waiting_time_per_lane = sum(self.get_waiting_time_per_lane())

        self.last_waiting_time = waiting_time_per_lane

        reward = 0
        proportion = self.get_halting_proportion()

        if proportion is None:
            self.last_reward = 0
        elif proportion < 0.25:
            reward = -4 * proportion + 1
        elif proportion >= 0.25:
            reward = -1.3333 * proportion + 0.3333

        self.last_reward = reward
        return self.last_reward

    def get_halting_proportion(self):
        total = 0
        halting = 0
        for l in self.lanes + self.out_lanes:
            total += self.sumo.lane.getLastStepVehicleNumber(l)
            halting += self.sumo.lane.getLastStepHaltingNumber(l)

        if total == 0:
            return None

        return halting / total

    def get_waiting_time_per_lane(self):
        wait_time_per_lane = []
        self.vehicles_wait_time = []
        for lane in self.lanes + self.out_lanes:
            vehicles = self.sumo.lane.getLastStepVehicleIDs(lane)
            wait_time = 0.0
            for veh in vehicles:
                veh_lane = self.sumo.vehicle.getLaneID(veh)
                acc = self.sumo.vehicle.getAccumulatedWaitingTime(veh)
                self.vehicles_wait_time.append(acc)
                if veh not in self.vehicles:
                    self.vehicles[veh] = {veh_lane: acc}
                else:
                    self.vehicles[veh][veh_lane] = acc - sum([self.vehicles[veh][lane] for lane in self.vehicles[veh].keys() if lane != veh_lane])
                wait_time += self.vehicles[veh][veh_lane]
            wait_time_per_lane.append(wait_time)
        return wait_time_per_lane

    def get_lanes_density(self):
        return [min(1, self.sumo.lane.getLastStepVehicleNumber(lane) / (self.lanes_lenght[lane] / self.vehicle_size_min_gap)) for lane in self.lanes]

    def get_lanes_queue(self):
        self.last_lanes_queue = sum([self.sumo.lane.getLastStepHaltingNumber(lane) for lane in self.lanes])
        return [min(1, self.sumo.lane.getLastStepHaltingNumber(lane) / (self.lanes_lenght[lane] / self.vehicle_size_min_gap)) for lane in self.lanes]

    def set_phase(self, phase, simulation_step):
        if self.green_phase == phase or self.time_since_last_phase_change < self.yellow_time + self.min_green_time:
            self.sumo.trafficlight.setRedYellowGreenState(self.id, self.phases[self.green_phase].state)
            self.next_action_time = simulation_step + self.delta_time
        else:
            self.sumo.trafficlight.setRedYellowGreenState(self.id, self.phases[self.yellow_dict[(self.green_phase, phase)]].state)
            self.green_phase = phase
            self.next_action_time = simulation_step + self.delta_time
            self.is_yellow = True
            self.time_since_last_phase_change = 0

    def update(self):
        self.time_since_last_phase_change += 1
        if self.is_yellow and self.time_since_last_phase_change == self.yellow_time:
            self.sumo.trafficlight.setRedYellowGreenState(self.id, self.phases[self.green_phase].state)
            self.is_yellow = False

    def is_time_to_act(self, simulation_step):
        return self.next_action_time <= simulation_step