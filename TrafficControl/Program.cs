using Microsoft.EntityFrameworkCore;
using TrafficControl.Persistence;
using TrafficControl.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TrafficControlContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IFileService, FileService>(sp => new FileService(sp.GetRequiredService<TrafficControlContext>(), builder.Configuration.GetValue<string>("FileBasePath")));
builder.Services.AddScoped<ScenarioService>();
builder.Services.AddScoped<SimulationService>(sp => new SimulationService(
    sp.GetRequiredService<TrafficControlContext>(), 
    sp.GetRequiredService<ScenarioService>(), 
    sp.GetRequiredService<IFileService>(),
    builder.Configuration.GetValue<string>("PythonExecutable"), 
    builder.Configuration.GetValue<string>("SimulationEntryPoint")));

builder.Services.AddCors();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
