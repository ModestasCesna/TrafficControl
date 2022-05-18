using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficControl.Persistence.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetworkFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RouteFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scenarios_Files_NetworkFileId",
                        column: x => x.NetworkFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Scenarios_Files_RouteFileId",
                        column: x => x.RouteFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimulationRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenarioId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Completed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationRuns_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationRunResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimulationRunId = table.Column<int>(type: "int", nullable: false),
                    Episode = table.Column<int>(type: "int", nullable: false),
                    IntersectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoppedVehiclesAverage = table.Column<float>(type: "real", nullable: false),
                    WaitingTimeAverage = table.Column<float>(type: "real", nullable: false),
                    Reward = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationRunResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationRunResults_SimulationRuns_SimulationRunId",
                        column: x => x.SimulationRunId,
                        principalTable: "SimulationRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_NetworkFileId",
                table: "Scenarios",
                column: "NetworkFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_RouteFileId",
                table: "Scenarios",
                column: "RouteFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationRunResults_SimulationRunId",
                table: "SimulationRunResults",
                column: "SimulationRunId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationRuns_ScenarioId",
                table: "SimulationRuns",
                column: "ScenarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimulationRunResults");

            migrationBuilder.DropTable(
                name: "SimulationRuns");

            migrationBuilder.DropTable(
                name: "Scenarios");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
