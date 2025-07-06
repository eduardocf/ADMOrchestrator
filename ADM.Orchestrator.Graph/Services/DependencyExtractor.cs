using System.Xml.Linq;

namespace ADM.Orchestrator.Graph.Services;

public class DependencyExtractor
{
	public async Task<Dictionary<string, List<string>>> ExtractDependenciesAsync()
	{
		var result = new Dictionary<string, List<string>>();
		foreach (var dir in Directory.GetDirectories("Repos"))
		{
			var project = Path.GetFileName(dir);
			var deps = new List<string>();

			foreach (var csproj in Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories))
			{
				var doc = XDocument.Load(csproj);
				deps.AddRange(
					doc.Descendants("PackageReference")
					   .Select(e => e.Attribute("Include")?.Value)
					   .Where(v => !string.IsNullOrEmpty(v))
					   .Select(v => v!)
				);
			}

			result[project] = deps.Distinct().ToList();
		}

		return await Task.FromResult(result);
	}
}