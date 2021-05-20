using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TacticView.Utilitiy
{
    public class AppInfo
    {
        public static string Token { get; set; } = null;

        public static string AccessToken { get; set; }
        public const string GITHUB_CLIENT_HEADER = "timheuer-microsoft-com";
        public static string GITHUB_CLIENT_ID { get; set; } = null;
        public static string GITHUB_CLIENT_SECRET { get; set; } = null;

        public AppInfo() { }

        private string _gitHash;
        public string GitHash
        {
            get
            {
                if (string.IsNullOrEmpty(_gitHash))
                {
                    var version = "1.0.0+a34a913742f8845d3da5309b7b17242222dlocal"; // Dummy version for local dev
                    var appAssembly = typeof(AppInfo).Assembly;
                    var infoVerAttr = appAssembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>()
                        .FirstOrDefault();

                    if (infoVerAttr != null && infoVerAttr.InformationalVersion.Length > 6)
                    {
                        // Hash is embedded in the version after a '+' symbol, e.g. 1.0.0+a34a913742f8845d3da5309b7b17242222d41a21
                        version = infoVerAttr.InformationalVersion;
                    }
                    _gitHash = version.Substring(version.IndexOf('+') + 1);
                }

                return _gitHash;
            }
        }

        public string ShortGitHash => GitHash.Substring(GitHash.Length - 6, 6);
    }
}
