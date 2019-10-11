using Humanizer;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
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
