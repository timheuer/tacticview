using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Octokit;

namespace TacticView.Data;

public class AppState
{
    private ProtectedLocalStorage _storage;

    public AppState(ProtectedLocalStorage localStorage)
    {
        _storage = localStorage;
    }

    public bool IsLoggedIn { get; private set; } = false;
    public string UserToken { get; private set; }
    public User User { get; set; }

    public async Task<bool> CheckIfAuthenticated()
    {
        var token = await _storage.GetAsync<string>("token");
        if (!string.IsNullOrEmpty(token.Value))
        {
            UserToken = token.Value;
            IsLoggedIn = true;
            return true;
        }
        return false;
    }
}
