using Microsoft.EntityFrameworkCore;

namespace TrafficControl.Persistence
{
    public class TrafficControlContext : DbContext
    {
        public TrafficControlContext(DbContextOptions<TrafficControlContext> options) : base(options)
        {
            
        }

        public DbSet<SimulationRun> SimulationRuns { get; set; } = default!;
        public DbSet<SimulationRunResult> SimulationRunResults { get; set; } = default!;
        public DbSet<File> Files { get; set; } = default!;
        public DbSet<Scenario> Scenarios { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var simulationRun = modelBuilder.Entity<SimulationRun>();
            simulationRun.HasKey(x => x.Id);
            simulationRun.Property(x => x.Name).IsRequired();
            simulationRun.Property(x => x.Created).IsRequired();
            simulationRun.HasMany(x => x.Results).WithOne(x => x.SimulationRun).HasForeignKey(x => x.SimulationRunId);
            simulationRun.HasOne(x => x.Scenario).WithMany(x => x.SimulationRuns).HasForeignKey(x => x.ScenarioId);

            var simulationRunResult = modelBuilder.Entity<SimulationRunResult>();
            simulationRunResult.HasKey(x => x.Id);
            simulationRunResult.Property(x => x.Episode).IsRequired();
            simulationRunResult.Property(x => x.IntersectionId).IsRequired();
            simulationRunResult.Property(x => x.Reward).IsRequired();
            simulationRunResult.Property(x => x.StoppedVehiclesAverage).IsRequired();
            simulationRunResult.Property(x => x.WaitingTimeAverage).IsRequired();

            var file = modelBuilder.Entity<File>();
            file.HasKey(x => x.Id);
            file.Property(x => x.FilePath).IsRequired();

            var scenario = modelBuilder.Entity<Scenario>();
            scenario.HasKey(x => x.Id);
            scenario.Property(x => x.Name).IsRequired();
            scenario.HasOne(x => x.NetworkFile).WithMany(x => x.NetworkScenarios).HasForeignKey(x => x.NetworkFileId).OnDelete(DeleteBehavior.NoAction);
            scenario.HasOne(x => x.RouteFile).WithMany(x => x.RouteScenarios).HasForeignKey(x => x.RouteFileId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}