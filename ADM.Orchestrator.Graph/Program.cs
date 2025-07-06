using ADM.Orchestrator.Graph.Data;
using ADM.Orchestrator.Graph.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
builder.Services.AddDbContext<GraphDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Services
builder.Services.AddHostedService<SchedulerService>();
builder.Services.AddScoped<RepoCloner>();
builder.Services.AddScoped<DependencyExtractor>();
builder.Services.AddScoped<DotBuilder>();
builder.Services.AddScoped<GraphRenderer>();
builder.Services.AddScoped<HtmlBuilder>();
builder.Services.AddScoped<ComparisonBuilder>();
builder.Services.AddScoped<DatabaseWriter>();

builder.Services.AddRazorPages();
var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();
app.Run();