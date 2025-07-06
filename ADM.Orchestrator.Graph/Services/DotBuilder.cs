using System.Text;

namespace ADM.Orchestrator.Graph.Services;

public class DotBuilder
{
	public string BuildDot(Dictionary<string, List<string>> graph)
	{
		var sb = new StringBuilder("digraph G {\n");
		foreach (var (app, deps) in graph)
		{
			foreach (var dep in deps)
				sb.AppendLine($"  \"{app}\" -> \"{dep}\";");
		}
		sb.Append("}");
		return sb.ToString();
	}
}