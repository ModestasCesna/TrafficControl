import argparse
import pandas as pd

from Agents.deep_q_learning import DeepQLearningAgent
from Environment.environment import Environment
from Persistence.persistence import Persistence

if __name__ == '__main__':

    argument_parser = argparse.ArgumentParser()
    argument_parser.add_argument("-n", dest="network")
    argument_parser.add_argument("-r", dest="route")
    argument_parser.add_argument("--id", dest="simulation_run_id")
    argument_parser.add_argument("--episodes", dest="episodes", default=40)
    args = argument_parser.parse_args()

    persistence = Persistence()

    env = Environment(args.network, args.route, simulation_time=21600)

    env.start_simulation(episode=1)
    states = env.get_state()

    agents = {i: DeepQLearningAgent(i, states[i], env.observation_space, env.action_space) for i in env.intersection_ids}

    for episode in range(1, args.episodes + 1):
        done = False

        while not done:
            actions = {ts: agents[ts].act() for ts in agents.keys()}
            observations, rewards, done = env.step(action=actions)

            for agent_id in agents:
                agents[agent_id].learn(observations[agent_id], rewards[agent_id])

        for agent_id in agents.keys():
            agents[agent_id].save_model(str(args.simulation_run_id), str(episode))

        df = pd.DataFrame(env.metrics)

        for intersection_id in env.intersections.keys():
            intersection_df = df[df["intersection_id"] == intersection_id]
            persistence.save_run_results(args.simulation_run_id, episode, intersection_id, intersection_df["total_stopped"].mean(),
                                         intersection_df["total_wait_time"].mean(), agents[intersection_id].reward)
            agents[intersection_id].reward = 0

        if episode < args.episodes:
            env.reset(episode=episode + 1)

    persistence.set_run_complete(args.simulation_run_id)