using System.Diagnostics;

namespace ADM.Orchestrator.Graph.Services;

public class GraphRenderer
{
	public string Render(string dot)
	{
		var dotPath = "graph.dot";
		var svgPath = "wwwroot/graph.svg";
		File.WriteAllText(dotPath, dot);

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "dot",
				Arguments = $"-Tsvg {dotPath} -o {svgPath}",
				RedirectStandardOutput = true,
				UseShellExecute = false
			}
		};

		process.Start();
		process.WaitForExit();
		return File.ReadAllText(svgPath);
	}
}