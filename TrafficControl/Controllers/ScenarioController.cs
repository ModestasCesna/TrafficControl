using Microsoft.AspNetCore.Mvc;
using TrafficControl.Models;
using TrafficControl.Service;

namespace TrafficControl.Controllers
{
    [ApiController]
    [Route("api/scenarios")]
    public class ScenarioController : ControllerBase
    {
        private readonly ScenarioService scenarioService;

        public ScenarioController(ScenarioService scenarioService)
        {
            this.scenarioService = scenarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetScenarios()
        {
            var scenarios = await scenarioService.GetAllScenarios();
            return Ok(scenarios.Select(x => new ScenarioResponse
            {
                Id = x.Id,
                Name = x.Name,
                Created = x.Created
            }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateScenario([FromForm]CreateScenarioRequest request)
        {
            await scenarioService.Create(request.Name, request.NetworkFile.OpenReadStream(), request.RouteFile.OpenReadStream());

            return NoContent();
        }
    }
}
