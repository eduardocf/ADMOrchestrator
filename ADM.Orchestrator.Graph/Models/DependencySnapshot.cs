namespace ADM.Orchestrator.Graph.Models;

public class DependencySnapshot
{
	public Guid Id { get; set; }
	public string ProjectName { get; set; } = "";
	public string DependenciesJson { get; set; } = "";
	public string GraphSvg { get; set; } = "";
	public DateTime Timestamp { get; set; }
}