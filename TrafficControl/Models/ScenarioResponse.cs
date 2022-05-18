namespace TrafficControl.Models
{
    public class ScenarioResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime Created { get; set; }
    }
}
