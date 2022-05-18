import sumolib
import traci
import numpy as np

from .intersection import Intersection


class Environment:
    def __init__(
        self,
        net: str,
        route: str,
        use_default_phases: bool = False,
        simulation_time: int = 10000,
        seed: int = 1
    ):
        self.net = net
        self.route = route

        self.use_default_phases = use_default_phases
        self.simulation_time = simulation_time
        self.seed = seed

        self.sumo_executable = sumolib.checkBinary('sumo')

        self.delta_time = 5
        self.teleport_count = 0

        self.metrics = []


    def start_simulation(self, episode):
        sumo_start_command = [
            self.sumo_executable,
            '-n', self.net,
            '-r', self.route,
            '--seed', str(self.seed) + str(episode),
            '--threads', str(4),
            '--start',
            '--quit-on-end'
        ]

        traci.start(sumo_start_command)
        self.sumo = traci

        self.intersection_ids = list(traci.trafficlight.getIDList())
        self.observations = {i: None for i in self.intersection_ids}
        self.rewards = {i: None for i in self.intersection_ids}

        self.intersections = {i: Intersection(self.sumo, i, self.delta_time, 3, self.use_default_phases) for i in self.intersection_ids}
        [self.intersections[i].setup() for i in self.intersection_ids]
        [self.intersections[i].build_phases() for i in self.intersection_ids]


    def execute_simulation_step(self):
        return self.sumo.simulationStep()

    def get_state(self):
        self.vehicles = dict()
        return self.compute_observations()

    def reset(self, episode):
        traci.close()
        self.sumo = None
        self.metrics = []
        self.teleport_count = 0

        self.start_simulation(episode)

    def compute_observations(self):
        self.observations.update({i: self.intersections[i].compute_observation() for i in self.intersection_ids if
                                    self.intersections[i].is_time_to_act(self.simulation_step)})

        return {i: self.observations[i] for i in self.observations.keys() if
                self.intersections[i].is_time_to_act(self.simulation_step)}

    def compute_rewards(self):
        self.rewards.update({i: self.intersections[i].compute_reward() for i in self.intersection_ids if
                                self.intersections[i].is_time_to_act(self.simulation_step)})

        return {i: self.rewards[i] for i in self.rewards.keys() if
                self.intersections[i].is_time_to_act(self.simulation_step)}

    def step(self, action):
        if action is None or action == {}:
            for _ in range(self.delta_time):
                self.execute_simulation_step()
        else:
            self.apply_actions(action)

            time_to_act = False
            while not time_to_act:
                self.execute_simulation_step()
                self.teleport_count += self.sumo.simulation.getEndingTeleportNumber()
                for intersection_id in self.intersection_ids:
                    self.intersections[intersection_id].update()
                    if self.intersections[intersection_id].is_time_to_act(self.simulation_step):
                        time_to_act = True

        observations = self.compute_observations()

        if self.teleport_count > 20:
            rewards = {i: self.teleport_count * -10 for i in self.rewards.keys() if
                    self.intersections[i].is_time_to_act(self.simulation_step)}
        else:
            rewards = self.compute_rewards()

        done = self.simulation_step > self.simulation_time or self.teleport_count > 10

        self.append_metrics()

        return observations, rewards, done

    def append_metrics(self):
        for intersection_id in self.intersections:
            self.metrics.append({
                'intersection_id': intersection_id,
                'simulation_step': self.simulation_step,
                'reward': self.intersections[intersection_id].last_reward,
                'total_stopped': self.intersections[intersection_id].last_lanes_queue,
                'total_wait_time': self.intersections[intersection_id].last_waiting_time
            })

    def apply_actions(self, actions):
        for intersection_id, action in actions.items():
            if self.intersections[intersection_id].is_time_to_act(self.simulation_step):
                self.intersections[intersection_id].set_phase(action, self.simulation_step)

    @property
    def simulation_step(self):
        return self.sumo.simulation.getTime()

    @property
    def observation_space(self):
        return self.intersections[self.intersection_ids[0]].observations

    @property
    def action_space(self):
        return self.intersections[self.intersection_ids[0]].actions
