namespace TrafficControl.Models
{
    public class StartSimulationRequest
    {
        public int ScenarioId { get; set; }
        public string Name { get; set; } = default!;
    }
}
