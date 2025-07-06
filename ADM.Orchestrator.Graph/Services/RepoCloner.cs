using LibGit2Sharp;

namespace ADM.Orchestrator.Graph.Services;

public class RepoCloner
{
	private readonly IConfiguration _config;
	private readonly ILogger<RepoCloner> _logger;

	public RepoCloner(IConfiguration config, ILogger<RepoCloner> logger)
	{
		_config = config;
		_logger = logger;
	}

	public async Task CloneReposAsync()
	{
		var repos = _config.GetSection("GitLab:Repositories").Get<string[]>() ?? [];
		var token = _config["GitLab:Token"];

		foreach (var repoUrl in repos)
		{
			var name = repoUrl.Split('/').Last().Replace(".git", "");
			var path = Path.Combine("Repos", name);

			if (Directory.Exists(path)) continue;

			var cloneOptions = new CloneOptions();
			cloneOptions.FetchOptions.CredentialsProvider = (_, _, _) => new UsernamePasswordCredentials
			{
				Username = "oauth2",
				Password = token
			};

			_logger.LogInformation($"Cloning {repoUrl}...");
			Repository.Clone(repoUrl, path, cloneOptions);
		}

		await Task.CompletedTask;
	}
}