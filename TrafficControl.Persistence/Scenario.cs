using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficControl.Persistence
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid? NetworkFileId { get; set; }
        public Guid? RouteFileId { get; set; }
        public DateTime Created { get; set; }

        public File? NetworkFile { get; set; } = default!;
        public File? RouteFile { get; set; } = default!;

        public ICollection<SimulationRun> SimulationRuns { get; set; } = default!;
    }
}
