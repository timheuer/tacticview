using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Octokit;
using TacticView.Utilitiy;

namespace TacticView.Data;

public class GitHubQueryService
{
    public static int? REQUESTS_PER_HOUR = 0;
    public static int? REQUESTS_LEFT = 0;
    public static string LIMIT_RESET;
    private List<Repo> _repos;
    private IWebHostEnvironment _env;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _config;

    public GitHubQueryService(IConfiguration configuration, IWebHostEnvironment environment, IDistributedCache cache)
    {
        var repos = configuration["REPO_LIST"].Split(',');
        _repos = new List<Repo>();
        _env = environment;
        _cache = cache;
        _config = configuration;

        foreach (var repo in repos)
        {
            var details = repo.Split('/');
            _repos.Add(new Repo() { Name = details[1], Owner = details[0] });
        }
    }

    public async Task<List<SimpleIssue>> GetPullRequestsAsIssuesAsync(string owner, string repo, string tag, bool openOnly = true)
    {
        // create the github client
        GitHubClient client = new GitHubClient(new ProductHeaderValue(AppInfo.GITHUB_CLIENT_HEADER));
        var basic = new Credentials(AppInfo.Token);
        client.Credentials = basic;

        // create the request parameters
        // using the tag to search for 
        RepositoryIssueRequest issueRequest = new();
        issueRequest.State = openOnly ? ItemStateFilter.Open : ItemStateFilter.All;
        issueRequest.Labels.Add(tag);

        // fetch all open pull requests
        var found = await client.Issue.GetAllForRepository(owner, repo, issueRequest);

        List<SimpleIssue> issues = new();
        foreach (var pr in found)
        {
            if (pr.PullRequest != null)
            {
                var labels = new List<Label>();
                foreach (var label in pr.Labels)
                {
                    labels.Add(new Label() { Color = label.Color, Name = label.Name });
                }

                var issue = new SimpleIssue()
                {
                    Number = pr.Number,
                    Title = pr.Title,
                    User = new User() { Login = pr.User.Login },
                    CreatedAt = pr.CreatedAt,
                    State = new State() { StringValue = pr.State.StringValue },
                    PullRequest = new PullRequest() { HtmlUrl = pr.PullRequest.HtmlUrl },
                    Labels = labels,
                    Repo = repo,
                    Org = owner
                };
                if (pr.Milestone != null)
                {
                    if (pr.Milestone.Title != null)
                        issue.Milestone = new Milestone() { Title = pr.Milestone.Title };
                }

                issues.Add(issue);
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

    public async Task GetReposAndIssuesAsync(string label, bool isOpenOnly = true)
    {
        ReposAndIssues thelist = new();
        TriageRepository allRepo = new TriageRepository() { Owner = "dotnet", Name = "All", Issues = new List<SimpleIssue>() };

        // query each repo for the label
        foreach (var repo in _repos)
        {
            // if it meets the condition, add to the model and get issues
            var issues = await GetPullRequestsAsIssuesAsync(repo.Owner, repo.Name, label, isOpenOnly);
            if (issues.Count > 0)
            {
                thelist.Repositories.Add(new TriageRepository() { Name = repo.Name, Issues = issues, Owner = repo.Owner });
                allRepo.Issues.AddRange(issues);
            };
        }
        if (thelist.Repositories.Count > 0)
            thelist.Repositories.Add(allRepo);

        await _cache.SetAsync(label, Serialize(thelist.Repositories));
        await _cache.SetStringAsync($"{label}-cacheDate", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

        // write to disk
        if (!String.IsNullOrEmpty(_config["DEBUG_REPO_DATA"]) && _config["DEBUG_REPO_DATA"].ToLowerInvariant() == "true")
        {
            using (var stream = File.Create(Path.Combine(_env.ContentRootPath, $"{label}.json")))
                await JsonSerializer.SerializeAsync(stream, thelist.Repositories, CreateOptions());
        }
    }

    public static byte[] Serialize<T>(List<T> list)
    {
        string jsonString = JsonSerializer.Serialize(list);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public static List<T> Deserialize<T>(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<List<T>>(jsonString);
    }

    public async Task<Tuple<List<TriageRepository>, DateTimeOffset>> GetCachedReposAndIssuesAsync(string label, bool isOpenOnly = true)
    {
        DateTimeOffset cacheDate;

        // get from the cache
        var cacheData = await _cache.GetAsync(label);
        
        if (cacheData == null)
        {
            await GetReposAndIssuesAsync(label, isOpenOnly);
            cacheData = await _cache.GetAsync(label);
        }

        var unixTimestamp = await _cache.GetStringAsync($"{label}-cacheDate");
        cacheDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToUInt32(unixTimestamp));
        var thelist = Deserialize<TriageRepository>(cacheData);
        return new Tuple<List<TriageRepository>, DateTimeOffset>(thelist, cacheDate);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            IncludeFields = true,
            MaxDepth = 30
        };

        return options;
    }

    public async Task<RateLimit> GetApiInfo()
    {
        // create the github client
        GitHubClient client = new GitHubClient(new ProductHeaderValue(AppInfo.GITHUB_CLIENT_HEADER));
        var basic = new Credentials(AppInfo.Token);
        client.Credentials = basic;

        var limits = await client.RateLimit.GetRateLimits();
        return limits.Rate;
    }
}
