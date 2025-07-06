using ADM.Orchestrator.Graph.Services;
using Microsoft.Extensions.Hosting;
using ADM.Orchestrator.Graph.Data;

namespace ADM.Orchestrator.Graph.Services;

public class SchedulerService : BackgroundService
{
	private readonly IServiceProvider _services;
	private readonly ILogger<SchedulerService> _logger;
	private readonly int? _intervalMinutes;
	private readonly TimeSpan? _fixedTime;

	public SchedulerService(IServiceProvider services, IConfiguration config, ILogger<SchedulerService> logger)
	{
		_services = services;
		_logger = logger;

		_intervalMinutes = config.GetValue<int?>("Scheduler:IntervalMinutes");

		var timeText = config.GetValue<string>("Scheduler:FixedTime");
		if (TimeSpan.TryParse(timeText, out var parsed))
			_fixedTime = parsed;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_intervalMinutes is not null)
			{
				await RunJobAsync(stoppingToken);
				await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes.Value), stoppingToken);
			}
			else if (_fixedTime is not null)
			{
				var now = DateTime.Now;
				var nextRun = now.Date.Add(_fixedTime.Value);
				if (nextRun <= now)
					nextRun = nextRun.AddDays(1);

				var delay = nextRun - now;
				_logger.LogInformation($"Next run scheduled at: {nextRun:u} ({delay.TotalMinutes:F1} mins)");
				await Task.Delay(delay, stoppingToken);

				await RunJobAsync(stoppingToken);
			}
			else
			{
				_logger.LogWarning("No scheduling configuration found. Waiting 1 hour before retry.");
				await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
			}
		}
	}

	private async Task RunJobAsync(CancellationToken stoppingToken)
	{
		using IServiceScope scope = _services.CreateScope();
		try
		{
			RepoCloner cloner = scope.ServiceProvider.GetRequiredService<RepoCloner>();
			DependencyExtractor extractor = scope.ServiceProvider.GetRequiredService<DependencyExtractor>();
			DotBuilder dot = scope.ServiceProvider.GetRequiredService<DotBuilder>();
			GraphRenderer renderer = scope.ServiceProvider.GetRequiredService<GraphRenderer>();
			HtmlBuilder html = scope.ServiceProvider.GetRequiredService<HtmlBuilder>();
			ComparisonBuilder compare = scope.ServiceProvider.GetRequiredService<ComparisonBuilder>();
			DatabaseWriter database = scope.ServiceProvider.GetRequiredService<DatabaseWriter>();

			await cloner.CloneReposAsync();
			List<DependencyExtractor.DependencySet> structuredDeps = await extractor.ExtractDependenciesAsync();
			string dotText = dot.BuildDot(structuredDeps);
			string svg = renderer.Render(dotText);
			string htmlPage = html.Build(structuredDeps, svg);
			string comparePage = compare.Build(structuredDeps);
			await database.SaveAsync(structuredDeps, svg);

			_logger.LogInformation($"Orchestration completed at {DateTime.UtcNow:u}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Scheduler orchestration failed");
		}
	}
}