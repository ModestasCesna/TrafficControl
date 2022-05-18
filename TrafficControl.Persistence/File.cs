using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficControl.Persistence
{
    public class File
    {
        public Guid Id { get; set; }
        public string FilePath { get; set; } = default!;

        public ICollection<Scenario> NetworkScenarios { get; set; } = default!;
        public ICollection<Scenario> RouteScenarios { get; set; } = default!;

    }
}
