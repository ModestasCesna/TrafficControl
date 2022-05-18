namespace TrafficControl.Models
{
    public class CreateScenarioRequest
    {
        public string Name { get; set; } = default!;
        public IFormFile NetworkFile { get; set; } = default!;
        public IFormFile RouteFile { get; set; } = default!;
    }
}
