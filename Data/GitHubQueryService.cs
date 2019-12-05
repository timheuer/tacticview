using Humanizer;
using Octokit;
using System.Collections.Generic;
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

        public async Task<IssueNotification> GetNotifications(string label)
        {
            var notifications = new IssueNotification();
            notifications.AspNetCore = NotificationIcon(await HasIssues("aspnet", "aspnetcore", label));
            notifications.CoreClr = NotificationIcon(await HasIssues("dotnet", "coreclr", label));
            notifications.CoreSetup = NotificationIcon(await HasIssues("dotnet", "core-setup", label));
            notifications.Cli = NotificationIcon(await HasIssues("dotnet", "cli", label));
            notifications.CoreFx = NotificationIcon(await HasIssues("dotnet", "corefx", label));
            notifications.Ef = NotificationIcon(await HasIssues("aspnet", "entityframeworkcore", label));
            notifications.Extensions = NotificationIcon(await HasIssues("aspnet", "extensions", label));
            notifications.MsBuild = NotificationIcon(await HasIssues("microsoft", "msbuild", label));
            notifications.Sdk = NotificationIcon(await HasIssues("dotnet", "sdk", label));
            notifications.Templating = NotificationIcon(await HasIssues("dotnet", "templating", label));
            notifications.Wcf = NotificationIcon(await HasIssues("dotnet", "wcf", label));
            notifications.WebSdk = NotificationIcon(await HasIssues("aspnet", "websdk", label));
            notifications.WinForms = NotificationIcon(await HasIssues("dotnet", "winforms", label));
            notifications.Wpf = NotificationIcon(await HasIssues("dotnet", "wpf", label));
            notifications.AspNetCoreTooling = NotificationIcon(await HasIssues("aspnet", "aspnetcore-tooling", label));

            return notifications;
        }
        private string NotificationIcon(bool hasIssues)
        {
            if (hasIssues) return "fiber_manual_record";
            return string.Empty;
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
