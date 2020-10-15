using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class ReposAndIssues
    {
        public List<TriageRepository> Repositories = new List<TriageRepository>();
    }

    public class TriageRepository
    {
        public List<Issue> Issues { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
    }

    public class Repo
    {
        public string Owner { get; set; }
        public string Name { get; set; }
    }
}
