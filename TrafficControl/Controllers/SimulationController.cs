using Microsoft.AspNetCore.Mvc;
using TrafficControl.Models;
using TrafficControl.Service;

namespace TrafficControl.Controllers
{
    [ApiController]
    [Route("api/simulations")]
    public class SimulationController : ControllerBase
    {
        private readonly SimulationService simulationService;

        public SimulationController(SimulationService simulationService)
        {
            this.simulationService = simulationService;
        }

        [HttpPost]
        public async Task<IActionResult> StartSimulation(StartSimulationRequest request)
        {
            await simulationService.StartSimulation(request.ScenarioId, request.Name);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var simulations = await simulationService.GetSimulations();

            return Ok(simulations.Select(s => new SimulationResponse(s)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var simulation = await simulationService.GetSimulation(id);

            return Ok(new SimulationResponse(simulation));
        }
    }
}