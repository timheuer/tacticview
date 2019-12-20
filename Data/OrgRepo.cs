using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class OrgRepo
    {
        public string Title { get; set; }
        public string OrgName { get; set; }
        public string RepoName { get; set; }
        public bool HasIssues { get; set; } = false;
    }
}
