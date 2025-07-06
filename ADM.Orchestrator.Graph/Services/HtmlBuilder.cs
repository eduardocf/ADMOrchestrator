using System.Text.Json;

namespace ADM.Orchestrator.Graph.Services;

public class HtmlBuilder
{
	public string Build(Dictionary<string, List<string>> graph, string svg)
	{
		var json = JsonSerializer.Serialize(graph, new JsonSerializerOptions { WriteIndented = true });
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
}