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

		var fixedTimeStr = config.GetValue<string>("Scheduler:FixedTime");
		if (TimeSpan.TryParse(fixedTimeStr, out var parsed))
			_fixedTime = parsed;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_intervalMinutes is not null)
			{
				await RunJob(stoppingToken);
				await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes.Value), stoppingToken);
			}
			else if (_fixedTime is not null)
			{
				var now = DateTime.Now;
				var nextRun = now.Date.Add(_fixedTime.Value);
				if (nextRun <= now)
					nextRun = nextRun.AddDays(1);

				var delay = nextRun - now;
				_logger.LogInformation($"Next run scheduled at: {nextRun:u} (in {delay.TotalMinutes:F1} minutes)");
				await Task.Delay(delay, stoppingToken);

				await RunJob(stoppingToken);
			}
			else
			{
				_logger.LogWarning("No valid scheduling configuration found. Skipping execution.");
				await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // fallback delay
			}
		}
	}

	private async Task RunJob(CancellationToken stoppingToken)
	{
		using var scope = _services.CreateScope();
		try
		{
			var cloner = scope.ServiceProvider.GetRequiredService<RepoCloner>();
			var extractor = scope.ServiceProvider.GetRequiredService<DependencyExtractor>();
			var dot = scope.ServiceProvider.GetRequiredService<DotBuilder>();
			var renderer = scope.ServiceProvider.GetRequiredService<GraphRenderer>();
			var html = scope.ServiceProvider.GetRequiredService<HtmlBuilder>();
			var compare = scope.ServiceProvider.GetRequiredService<ComparisonBuilder>();
			var db = scope.ServiceProvider.GetRequiredService<DatabaseWriter>();

			await cloner.CloneReposAsync();
			var deps = await extractor.ExtractDependenciesAsync();
			var dotSrc = dot.BuildDot(deps);
			var svg = renderer.Render(dotSrc);
			var htmlPg = html.Build(deps, svg);
			var cmpPg = compare.Build(deps);
			await db.SaveAsync(deps, svg);

			_logger.LogInformation($"Graph orchestration completed at: {DateTime.UtcNow:u}");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Orchestration job failed.");
		}
	}
}