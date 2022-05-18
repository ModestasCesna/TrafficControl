using TrafficControl.Persistence;

namespace TrafficControl.Models
{
    public class SimulationResultResponse
    {
        public SimulationResultResponse(SimulationRunResult simulationRunResult)
        { 
            Episode = simulationRunResult.Episode;
            IntersectionId = simulationRunResult.IntersectionId;
            StoppedVehiclesAverage = simulationRunResult.StoppedVehiclesAverage;
            WaitingTimeAverage = simulationRunResult.WaitingTimeAverage;
            Reward = simulationRunResult.Reward;
        }

        public int Episode { get; set; }
        public string IntersectionId { get; set; } = default!;
        public float StoppedVehiclesAverage { get; set; }
        public float WaitingTimeAverage { get; set; }
        public float Reward { get; set; }
    }
}
