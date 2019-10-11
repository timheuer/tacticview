using Blazored.LocalStorage;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TacticView.Data
{
    public class AppState
    {
        private ILocalStorageService _storage;

        public AppState(ILocalStorageService localStorage) 
        {
            _storage = localStorage;
        }

        public bool IsLoggedIn { get; private set; } = false;
        public string UserToken { get; private set; }
        public User User { get; set; }

        public async Task<bool> CheckIfAuthenticated()
        {
            var token = await _storage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                UserToken = token;
                IsLoggedIn = true;
                return true;
            }
            return false;
        }
    }
}
