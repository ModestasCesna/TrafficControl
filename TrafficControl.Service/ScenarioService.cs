using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficControl.Persistence;

namespace TrafficControl.Service
{
    public class ScenarioService
    {
        private readonly TrafficControlContext dbContext;
        private readonly IFileService fileService;

        public ScenarioService(TrafficControlContext dbContext, IFileService fileService)
        {
            this.dbContext = dbContext;
            this.fileService = fileService;
        }

        public async Task Create(string name, Stream networkFile, Stream routeFile)
        {
            var networkFileId = await fileService.Store("network.net.xml", networkFile);
            var routeFileId = await fileService.Store("route.net.xml", routeFile);

            dbContext.Scenarios.Add(new Scenario
            {
                Name = name,
                NetworkFileId = networkFileId,
                RouteFileId = routeFileId,
                Created = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Scenario>> GetAllScenarios()
        {
            return await dbContext.Scenarios.OrderByDescending(s => s.Created).ToListAsync();
        }

        public async Task<Scenario> GetScenario(int scenarioId)
        {
            return await dbContext.Scenarios.SingleAsync(x => x.Id == scenarioId);
        }
    }
}
