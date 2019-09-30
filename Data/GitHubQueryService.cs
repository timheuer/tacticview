using Humanizer;
using Octokit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class GitHubQueryService
    {
        const string PRODUCT_HEADER = "timheuer-microsoft-com";
        public static int? REQUESTS_PER_HOUR = 0;
        public static int? REQUESTS_LEFT = 0;
        public static string LIMIT_RESET;

        public async Task<List<Issue>> GetOpenPullRequestsAsIssuesAsync(string owner, string repo, string tag)
        {
            List<Issue> issues = new List<Issue>();

            // create the github client
            GitHubClient client = new GitHubClient(new ProductHeaderValue(PRODUCT_HEADER));
            var basic = new Credentials(Startup.Token);
            client.Credentials = basic;

            // create the request parameters
            // using the tag to search for 
            var issueRequest = new RepositoryIssueRequest();
            issueRequest.State = ItemStateFilter.Open;
            issueRequest.Labels.Add(tag);

            // fetch all open pull requests
            var found = await client.Issue.GetAllForRepository(owner, repo, issueRequest);

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
            //Console.WriteLine($"LIMIT: {REQUESTS_PER_HOUR}; LEFT: {REQUESTS_LEFT}; RESET: {LIMIT_RESET}");

            return issues;
        }

        public async Task<RateLimit> GetApiInfo()
        {
            // create the github client
            GitHubClient client = new GitHubClient(new ProductHeaderValue(PRODUCT_HEADER));
            var basic = new Credentials(Startup.Token);
            client.Credentials = basic;

            var limits = await client.Miscellaneous.GetRateLimits();
            return limits.Rate;
        }
    }
}
