namespace ADM.Orchestrator.Graph.Services;

public class DependencyExtractor
{
	public class DependencySet
	{
		public string Project { get; set; } = "";
		public List<string> Nuget { get; set; } = new();
		public List<string> Dll { get; set; } = new();
	}

	public async Task<List<DependencySet>> ExtractDependenciesAsync()
	{
		var results = new List<DependencySet>();

		foreach (var dir in Directory.GetDirectories("Repos"))
		{
			var name = Path.GetFileName(dir);
			var nuget = new HashSet<string>();
			var dll = new HashSet<string>();

			foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
			{
				if (file.EndsWith(".csproj"))
				{
					var doc = System.Xml.Linq.XDocument.Load(file);

					nuget.UnionWith(doc.Descendants("PackageReference")
						.Select(x => x.Attribute("Include")?.Value)
						.Where(x => !string.IsNullOrWhiteSpace(x))!);

					dll.UnionWith(doc.Descendants("Reference")
						.Select(x => x.Attribute("Include")?.Value)
						.Where(x => !string.IsNullOrWhiteSpace(x))!);

					dll.UnionWith(doc.Descendants("COMReference")
						.Select(x => x.Attribute("Include")?.Value)
						.Where(x => !string.IsNullOrWhiteSpace(x))!);
				}
			}

			results.Add(new DependencySet
			{
				Project = name,
				Nuget = nuget.OrderBy(x => x).ToList(),
				Dll = dll.OrderBy(x => x).ToList()
			});
		}

		return await Task.FromResult(results);
	}
}