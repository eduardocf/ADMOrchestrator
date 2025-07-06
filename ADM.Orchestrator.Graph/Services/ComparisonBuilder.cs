using System.Text;

namespace ADM.Orchestrator.Graph.Services;

public class ComparisonBuilder
{
	public string Build(Dictionary<string, List<string>> graph)
	{
		var html = new StringBuilder("<html><body><h2>Dependency Comparison</h2><ul>");
		foreach (var (app, deps) in graph)
		{
			html.AppendLine($"<li><strong>{app}</strong>: {string.Join(", ", deps)}</li>");
		}
		html.Append("</ul></body></html>");
		File.WriteAllText("wwwroot/compare.html", html.ToString());
		return html.ToString();
	}
}