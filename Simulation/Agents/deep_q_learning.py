import numpy as np
import tensorflow as tf
from tensorflow import keras
from tensorflow.keras import layers
import os


class DeepQLearningAgent:
    def __init__(self, id, state, states, actions, epsilon=1.00, min_epsilon=0.0, alpha=0.1, gamma=0.99, decay=0.999):
        self.id = id
        self.state = state
        self.states = states
        self.actions = actions
        self.epsilon = epsilon
        self.min_epsilon = min_epsilon
        self.alpha = alpha
        self.gamma = gamma
        self.decay = decay
        self.reward = 0
        self.batch_size = 32

        self.max_historical_data = 1000000

        self.action_history = []
        self.state_history = []
        self.state_next_history = []
        self.rewards_history = []

        lr_schedule = keras.optimizers.schedules.ExponentialDecay(
            initial_learning_rate=0.000025,
            decay_steps=4320,
            decay_rate=0.9)

        self.optimizer = keras.optimizers.Adam(learning_rate=lr_schedule, clipnorm=1.0)
        self.loss_function = keras.losses.Huber()

        self.model = self._create_keras_model()

    def act(self):
        if np.random.rand() < self.epsilon:
            self.action = int(self.actions.sample())
        else:
            state_tensor = tf.convert_to_tensor(self.state)
            state_tensor = tf.expand_dims(state_tensor, 0)
            action_probabilities = self.model(state_tensor, training=False)
            self.action = np.argmax(action_probabilities[0])

        self.epsilon = max(self.epsilon * self.decay, self.min_epsilon)

        return self.action

    def learn(self, next_state, reward):
        self.reward += reward

        self.action_history.append(self.action)
        self.state_history.append(self.state)
        self.state_next_history.append(next_state)
        self.rewards_history.append(reward)

        self.state = next_state

        if len(self.rewards_history) % 4 == 0 and len(self.rewards_history) >= self.batch_size:
            indices = np.random.choice(range(len(self.rewards_history)), size=self.batch_size)

            state_sample = np.array([self.state_history[i] for i in indices])
            state_next_sample = np.array([self.state_next_history[i] for i in indices])
            reward_sample = [self.rewards_history[i] for i in indices]
            action_sample = [self.action_history[i] for i in indices]

            future_rewards = self.model.predict(state_next_sample)

            updated_q_values = reward_sample + self.gamma * tf.reduce_max(future_rewards, axis=1)

            masks = tf.one_hot(action_sample, self.actions.n)

            with tf.GradientTape() as tape:
                q_values = self.model(state_sample)
                q_action = tf.reduce_sum(tf.multiply(q_values, masks), axis=1)
                loss = self.loss_function(updated_q_values, q_action)

            grads = tape.gradient(loss, self.model.trainable_variables)
            self.optimizer.apply_gradients(zip(grads, self.model.trainable_variables))

        if len(self.rewards_history) > self.max_historical_data:
            del self.action_history[:128]
            del self.state_history[:128]
            del self.state_next_history[:128]
            del self.rewards_history[:128]


    def _create_keras_model(self):
        return keras.Sequential(
            [
                layers.Input(shape=(len(self.state),)),
                layers.Dense(256, activation="relu"),
                layers.Dense(256, activation="relu"),
                layers.Dense(256, activation="relu"),
                layers.Dense(self.actions.n, activation="linear")
            ]
        )

    def save_model(self, run_id, episode):
        path = "C:/t/runs/" + run_id + "/" + episode + "/" + self.id + "/"
        self.model.save(path)