using static ADM.Orchestrator.Graph.Services.DependencyExtractor;

namespace ADM.Orchestrator.Graph.Services;

public class ComparisonBuilder
{
	public string Build(List<DependencySet> deps)
	{
		var sb = new System.Text.StringBuilder("<html><body><h2>Dependency Comparison</h2><ul>");

		foreach (var d in deps)
		{
			var nugets = d.Nuget.Count > 0 ? string.Join(", ", d.Nuget) : "None";
			var dlls = d.Dll.Count > 0 ? string.Join(", ", d.Dll) : "None";

			sb.AppendLine($"<li><strong>{d.Project}</strong> NuGet: {nugets} | DLL: {dlls}</li>");
		}

		sb.AppendLine("</ul></body></html>");
		File.WriteAllText("wwwroot/compare.html", sb.ToString());
		return sb.ToString();
	}
}