import numpy as np

class EpsilonGreedyAgent:
    def __init__(self, state, states, actions, epsilon=1.00, min_epsilon=0.0, alpha=0.1, gamma=0.99, decay=0.99):
        self.state = state
        self.states = states
        self.actions = actions
        self.epsilon = epsilon
        self.min_epsilon = min_epsilon
        self.alpha = alpha
        self.gamma = gamma
        self.decay = decay
        self.reward = 0

        self.q_table = {self.state: [0 for _ in range(actions.n)]}

    def act(self):
        if np.random.rand() < self.epsilon:
            self.action = int(self.actions.sample())
        else:
            self.action = np.argmax(self.q_table[self.state])

        self.epsilon = max(self.epsilon * self.decay, 0)

        return self.action

    def learn(self, next_state, reward):
        if next_state not in self.q_table:
            self.q_table[next_state] = [0 for _ in range(self.actions.n)]

        self.q_table[self.state][self.action] = \
            self.q_table[self.state][self.action] \
            + self.alpha * (reward + self.gamma * max(self.q_table[next_state])
            - self.q_table[self.state][self.action])

        self.state = next_state
        self.reward += reward