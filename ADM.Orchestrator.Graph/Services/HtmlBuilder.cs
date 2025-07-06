using static ADM.Orchestrator.Graph.Services.DependencyExtractor;

namespace ADM.Orchestrator.Graph.Services;

public class HtmlBuilder
{
	public string Build(List<DependencySet> deps, string svg)
	{
		var jsonObjects = deps.Select(d => new
		{
			project = d.Project,
			nuget = Format(d.Nuget),
			dll = Format(d.Dll)
		}).ToList();

		var json = System.Text.Json.JsonSerializer.Serialize(jsonObjects, new System.Text.Json.JsonSerializerOptions
		{
			WriteIndented = true
		});

		File.WriteAllText("wwwroot/dependencies.json", json);

		var html = $"""
        <html><body>
        <h1>Dependency Graph</h1>
        {svg}
        <pre>{json}</pre>
        </body></html>
        """;

		File.WriteAllText("wwwroot/index.html", html);
		return html;
	}

	private object? Format(List<string> list)
	{
		if (list.Count == 0) return null;
		if (list.Count == 1) return list[0];
		return list;
	}
}