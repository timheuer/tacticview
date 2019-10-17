using Microsoft.AspNetCore.Components;
using Octokit;
using Radzen;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TacticView.Shared
{
    public partial class GridList
    {
        List<Issue> issues;

        [Parameter]
        public string OrgName { get; set; }

        [Parameter]
        public string RepoName { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter] public bool IsOpen { get; set; }

        [Parameter] public bool EnableApproval { get; set; } = false;

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                issues = await IssuesService.GetPullRequestsAsIssuesAsync(OrgName, RepoName, Label, IsOpen);
                StateHasChanged();
            }
        }

        private async Task ApproveItem(int issueNumber)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Not Implemented", "Some testing still to do, not yet implemented", 2000);
            await InvokeAsync(() => StateHasChanged());
            //Console.WriteLine(issueNumber);

            //var client = new GitHubClient(new ProductHeaderValue(Startup.GITHUB_CLIENT_HEADER));
            //var creds = new Credentials(AppState.UserToken);
            //client.Credentials = creds;

            //var itemUpdate = new IssueUpdate();
            //itemUpdate.AddLabel("servicing-approved");

            //try
            //{
            //    var x = await client.Issue.Update("dotnet", "coreclr", 27080, itemUpdate);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
