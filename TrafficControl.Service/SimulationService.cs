using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using TrafficControl.Persistence;

namespace TrafficControl.Service
{
    public class SimulationService
    {
        private readonly TrafficControlContext dbContext;
        private readonly ScenarioService scenarioService;
        private readonly IFileService fileService;
        private readonly string pythonPath;
        private readonly string simulationEntryFile;

        public SimulationService(TrafficControlContext dbContext, ScenarioService scenarioService, IFileService fileService, string pythonPath, string simulationEntryFile)
        {
            this.dbContext = dbContext;
            this.scenarioService = scenarioService;
            this.fileService = fileService;
            this.pythonPath = pythonPath;
            this.simulationEntryFile = simulationEntryFile;
        }

        public async Task StartSimulation(int scenarioId, string name)
        {
            var scenario = await scenarioService.GetScenario(scenarioId);

            var networkFilePath = await fileService.GetPath(scenario.NetworkFileId!.Value);
            var routesFilePath = await fileService.GetPath(scenario.RouteFileId!.Value);

            var simulationRun = new SimulationRun
            {
                Scenario = scenario,
                Name = name,
                State = SimulationRunState.Running,
                Created = DateTime.UtcNow
            };

            dbContext.Add(simulationRun);
            await dbContext.SaveChangesAsync();

            RunSimulation(networkFilePath, routesFilePath, simulationRun.Id);
        }

        public async Task<IEnumerable<SimulationRun>> GetSimulations()
        {
            var simulations = await dbContext.SimulationRuns
                .OrderByDescending(sr => sr.Created)
                .Include(sr => sr.Scenario)
                .ToListAsync();

            return simulations;
        }

        public async Task<SimulationRun> GetSimulation(int id)
        {
            var simulation = await dbContext.SimulationRuns
                .Include(sr => sr.Scenario)
                .Include(sr => sr.Results)
                .SingleAsync(sr => sr.Id == id);

            return simulation;
        }

        private void RunSimulation(string networkFile, string routesFile, int simulationRunId)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = string.Join(" ", new string[]
                    {
                        simulationEntryFile,
                        "-n", networkFile,
                        "-r", routesFile,
                        "--id", simulationRunId.ToString()
                    })
            };

            Process.Start(processStartInfo);
        }
    }
}