using ADM.Orchestrator.Graph.Data;
using ADM.Orchestrator.Graph.Models;
using static ADM.Orchestrator.Graph.Services.DependencyExtractor;

namespace ADM.Orchestrator.Graph.Services;

public class DatabaseWriter
{
	private readonly GraphDbContext _db;

	public DatabaseWriter(GraphDbContext db)
	{
		_db = db;
	}

	public async Task SaveAsync(List<DependencySet> deps, string svg)
	{
		foreach (var d in deps)
		{
			var json = System.Text.Json.JsonSerializer.Serialize(new
			{
				project = d.Project,
				nuget = d.Nuget,
				dll = d.Dll
			});

			_db.Snapshots.Add(new DependencySnapshot
			{
				Id = Guid.NewGuid(),
				ProjectName = d.Project,
				DependenciesJson = json,
				GraphSvg = svg,
				Timestamp = DateTime.UtcNow
			});
		}

		await _db.SaveChangesAsync();
	}
}