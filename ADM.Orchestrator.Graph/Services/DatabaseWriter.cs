using ADM.Orchestrator.Graph.Data;
using ADM.Orchestrator.Graph.Models;
using System.Text.Json;

namespace ADM.Orchestrator.Graph.Services;

public class DatabaseWriter
{
	private readonly GraphDbContext _db;

	public DatabaseWriter(GraphDbContext db) => _db = db;

	public async Task SaveAsync(Dictionary<string, List<string>> graph, string svg)
	{
		foreach (var (project, deps) in graph)
		{
			var snapshot = new DependencySnapshot
			{
				Id = Guid.NewGuid(),
				ProjectName = project,
				DependenciesJson = JsonSerializer.Serialize(deps),
				GraphSvg = svg,
				Timestamp = DateTime.UtcNow
			};
			_db.Snapshots.Add(snapshot);
		}
		await _db.SaveChangesAsync();
	}
}