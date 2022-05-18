using TrafficControl.Persistence;

namespace TrafficControl.Models
{
    public class SimulationResponse
    {
        public SimulationResponse(SimulationRun simulationRun)
        {
            Id = simulationRun.Id;
            Name = simulationRun.Name;
            ScenarioId = simulationRun.ScenarioId;
            ScenarioName = simulationRun.Scenario.Name;
            State = simulationRun.State;
            Created = simulationRun.Created;
            Completed = simulationRun.Completed;

            if(simulationRun.Results != null)
            {
                Results = simulationRun.Results.Select(r => new SimulationResultResponse(r));
            }
        }

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int ScenarioId { get; set; }
        public string ScenarioName { get; set; }
        public SimulationRunState State { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        public IEnumerable<SimulationResultResponse> Results { get; set; } = default!;
    }
}
