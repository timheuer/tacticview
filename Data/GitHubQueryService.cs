using Humanizer;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class GitHubQueryService
    {
        public static int? REQUESTS_PER_HOUR = 0;
        public static int? REQUESTS_LEFT = 0;
        public static string LIMIT_RESET;

        public async Task<List<Issue>> GetPullRequestsAsIssuesAsync(string owner, string repo, string tag, bool openOnly=true)
        {
            // create the github client
            GitHubClient client = new GitHubClient(new ProductHeaderValue(Startup.GITHUB_CLIENT_HEADER));
            var basic = new Credentials(Startup.Token);
            client.Credentials = basic;

            // create the request parameters
            // using the tag to search for 
            var issueRequest = new RepositoryIssueRequest();
            issueRequest.State = openOnly ? ItemStateFilter.Open : ItemStateFilter.All;
            issueRequest.Labels.Add(tag);

            // fetch all open pull requests
            var found = await client.Issue.GetAllForRepository(owner, repo, issueRequest);

            List<Issue> issues = new List<Issue>();
            foreach (var pr in found)
            {
                if (pr.PullRequest != null)
                {
                    issues.Add(pr);
                }
            }

            // get some rate limit info
            var apiInfo = client.GetLastApiInfo();
            var rateLimit = apiInfo?.RateLimit;
            REQUESTS_PER_HOUR = rateLimit?.Limit;
            REQUESTS_LEFT = rateLimit?.Remaining;
            LIMIT_RESET = rateLimit?.Reset.Humanize();

            return issues;
        }
        
        public async Task<List<OrgRepo>> GetReposWithIssues(string label)
        {
            var repos = await GetRepos();

            foreach (var item in repos)
            {
                item.HasIssues = await HasIssues(item.OrgName, item.RepoName, label);
            }

            return repos;
        }

        private async Task<List<OrgRepo>> GetRepos()
        {
            List<OrgRepo> repos = new List<OrgRepo>();

            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "aspnetcore", Title = "AspNetCore" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "blazor", Title = "Blazor" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "coreclr", Title = "CoreCLR" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "corefx", Title = "CoreFX" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "core-setup", Title = "Setup" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "cli", Title = "CLI" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "efcore", Title = "EF" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "extensions", Title = "Extensions" });
            repos.Add(new OrgRepo() { OrgName = "microsoft", RepoName = "msbuild", Title = "MSBuild" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "sdk", Title = "SDK" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "templating", Title = "Templating" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "wcf", Title = "WCF" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "winforms", Title = "WinForms" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "wpf", Title = "WPF" });
            repos.Add(new OrgRepo() { OrgName = "aspnet", RepoName = "websdk", Title = "Web SDK" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "aspnetcore-tooling", Title = "ASPNET Core Tooling" });
            repos.Add(new OrgRepo() { OrgName = "dotnet", RepoName = "runtime", Title = "Runtime" });

            return repos;
        }

        // TODO: Quick hack duplicate of previous function...refactor this garbage
        public async Task<bool> HasIssues(string owner, string repo, string tag, bool openOnly=true)
        {
            var repoHasIssues = false;

            // create the github client
            GitHubClient client = new GitHubClient(new ProductHeaderValue(Startup.GITHUB_CLIENT_HEADER));
            var basic = new Credentials(Startup.Token);
            client.Credentials = basic;

            // create the request parameters
            // using the tag to search for 
            var issueRequest = new RepositoryIssueRequest();
            issueRequest.State = openOnly ? ItemStateFilter.Open : ItemStateFilter.All;
            issueRequest.Labels.Add(tag);

            // fetch all open pull requests
            var found = await client.Issue.GetAllForRepository(owner, repo, issueRequest);

            foreach (var pr in found)
            {
                if (pr.PullRequest != null)
                {
                    repoHasIssues = true;
                    break;
                }
            }

            return repoHasIssues;
        }

        public async Task<RateLimit> GetApiInfo()
        {
            // create the github client
            GitHubClient client = new GitHubClient(new ProductHeaderValue(Startup.GITHUB_CLIENT_HEADER));
            var basic = new Credentials(Startup.Token);
            client.Credentials = basic;

            var limits = await client.Miscellaneous.GetRateLimits();
            return limits.Rate;
        }
    }
}
