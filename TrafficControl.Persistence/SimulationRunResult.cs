using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficControl.Persistence
{
    public class SimulationRunResult
    {
        public int Id { get; set; }
        public int SimulationRunId { get; set; }
        public int Episode { get; set; }
        public string IntersectionId { get; set; } = default!;
        public float StoppedVehiclesAverage { get; set; }
        public float WaitingTimeAverage { get; set; }
        public float Reward { get; set; }

        public SimulationRun SimulationRun { get; set; } = default!;
    }
}
