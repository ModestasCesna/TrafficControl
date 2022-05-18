import pyodbc

class Persistence:
    def __init__(self):
        self.connection = pyodbc.connect(Trusted_Connection='yes', driver = '{SQL Server}', server = '.\\sqlexpress' , database = 'TrafficControlDb')

    def save_run_results(self, run_id, episode, intersection_id, stopped_vehicles_average, waiting_time_average, reward):
        cursor = self.connection.cursor()
        cmd = "INSERT INTO [SimulationRunResults] " \
                  "([SimulationRunId], [Episode], [IntersectionId], [StoppedVehiclesAverage], [WaitingTimeAverage], [Reward]) " \
                  "VALUES (?, ?, ?, ?, ?, ?)"

        cursor.execute(cmd, run_id, episode, intersection_id, stopped_vehicles_average, waiting_time_average, reward)
        cursor.commit()

    def set_run_complete(self, simulation_run_id):
        cursor = self.connection.cursor()
        cmd = "UPDATE [SimulationRuns] SET [State] = 2 WHERE [Id] = ?"
        cursor.execute(cmd, simulation_run_id)
        cursor.commit()