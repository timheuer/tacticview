using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class ReposAndIssues
    {
        public List<TriageRepository> Repositories = new();
    }

    public class TriageRepository
    {
        public List<SimpleIssue> Issues { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
    }

    public class Repo
    {
        public string Owner { get; set; }
        public string Name { get; set; }
    }

    public class SimpleIssue
    {
        public string Title { get; set; }
        public int Number { get; set; }

        public PullRequest PullRequest { get; set; }

        public Milestone Milestone { get; set; }

        public State State { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<Label> Labels { get; set; }
        public User User { get; set; }
        public string Repo { get; set; }
        public string Org { get; set; }

    }

    public class State
    {
        public string StringValue { get; set; }
    }

    public class PullRequest
    {
        public string HtmlUrl { get; set; }
    }

    public class Milestone
    {
        public string Title { get; set; }
    }

    public class Label
    {
        public string Color { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string Login { get; set; }
    }
}
