using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace TacticView.Shared
{
    public partial class GitHubAuth
    {
        public string LoginUrl { get; set; }
        public bool IsAuthenticated { get; set; }

        void GetLoginUrl()
        {
            var client = new GitHubClient(new ProductHeaderValue(Startup.GITHUB_CLIENT_HEADER));
            var request = new OauthLoginRequest(Startup.GITHUB_CLIENT_ID)
            {
                State = Guid.NewGuid().ToString(),
                Scopes = { "repo" }
            };

            LoginUrl = client.Oauth.GetGitHubLoginUrl(request).ToString();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            GetLoginUrl();
            IsAuthenticated = await AppState.CheckIfAuthenticated();
            StateHasChanged();
        }
    }
}
