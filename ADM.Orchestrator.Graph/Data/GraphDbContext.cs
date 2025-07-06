using ADM.Orchestrator.Graph.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ADM.Orchestrator.Graph.Data;

public class GraphDbContext : DbContext
{
	public GraphDbContext(DbContextOptions<GraphDbContext> options) : base(options) { }

	public DbSet<DependencySnapshot> Snapshots => Set<DependencySnapshot>();
}