namespace ADM.Orchestrator.Graph.Services;

using static ADM.Orchestrator.Graph.Services.DependencyExtractor;

public class DotBuilder
{
	public string BuildDot(List<DependencySet> deps)
	{
		var sb = new System.Text.StringBuilder();
		sb.AppendLine("digraph Dependencies {");
		sb.AppendLine("  node[shape=ellipse style=\"rounded,filled\" color=\"lightgoldenrodyellow\" fontname=\"Arial\"];");
		sb.AppendLine("  rankdir=LR;");

		var allApps = deps.Select(d => d.Project).ToHashSet();

		foreach (var set in deps)
		{
			sb.AppendLine($"  \"{set.Project}\" [shape=ellipse, color=\"lightgoldenrodyellow\"];");

			foreach (var pkg in set.Nuget)
			{
				sb.AppendLine($"  \"{set.Project}\" -> \"{pkg}\" [color=black];");
				sb.AppendLine($"  \"{pkg}\" [shape=box, color=\"#e6f0ff\"];");
			}

			foreach (var dll in set.Dll)
			{
				var isApp = allApps.Contains(dll);
				var shape = "ellipse";
				var color = "lightgoldenrodyellow";
				var edgeColor = isApp ? "blue" : "black";

				sb.AppendLine($"  \"{set.Project}\" -> \"{dll}\" [color={edgeColor}];");
				sb.AppendLine($"  \"{dll}\" [shape={shape}, color=\"{color}\"];");
			}
		}

		sb.AppendLine("}");
		return sb.ToString();
	}
}