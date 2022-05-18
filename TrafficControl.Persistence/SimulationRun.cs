using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficControl.Persistence
{
    public class SimulationRun
    {
        public int Id { get; set; }
        public int ScenarioId { get; set; }
        public string Name { get; set; } = default!;
        public SimulationRunState State { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }

        public ICollection<SimulationRunResult> Results { get; set; } = default!;
        public Scenario Scenario { get; set; } = default!;

    }

    public enum SimulationRunState
    {
        Running = 1,
        Completed = 2
    }
}
