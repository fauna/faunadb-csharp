using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaunaDB.Client.Utils
{
    public class CheckLatestVersion
    {
        private const string packageName = "FaunaDB.Client";
        private static bool? alreadyChecked = null;
        public static async Task GetVersionAsync()
        {
            if (alreadyChecked.HasValue) return;
            var latestNuGetVesrionString = string.Empty;
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageName}/index.json";
            try
            {
                var httpClient = new HttpClient();
#if NET45
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
#endif
                var response = await httpClient.GetAsync(url);
                string versionsResponse = await response.Content.ReadAsStringAsync();
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(versionsResponse);
                var latestNuGetVesrion = ((Newtonsoft.Json.Linq.JArray)jObject.First.First).Children().LastOrDefault();
                latestNuGetVesrionString = latestNuGetVesrion.ToString();
                alreadyChecked = true;
            }
            catch(Exception ex)
            {
                alreadyChecked = false;
                var message = $"Enable to check new Fauna driver version. Exception: {ex.Message}";
                return;
            }

            Assembly asm = typeof(CheckLatestVersion).GetTypeInfo().Assembly;
            var currentVersion =  asm.GetName().Version;
            var currentVersionString = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
            if (!latestNuGetVesrionString.Equals(currentVersionString))
            {
                var message =  GetMessage(latestNuGetVesrionString, currentVersionString);
                Debug.WriteLine(message);
                System.Console.WriteLine(message);
            }
        }

        private static string GetMessage(string latestNuGetVesrion, string currentVersion)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("".PadLeft(80, '='));
            sb.Append(System.Environment.NewLine);
            sb.Append($"New fauna version available {currentVersion} -> {latestNuGetVesrion}");
            sb.Append(System.Environment.NewLine);
            sb.Append($"Changelog: https://github.com/fauna/faunadb-csharp/blob/master/CHANGELOG.md");
            sb.Append(System.Environment.NewLine);
            sb.Append("".PadLeft(80, '='));
            return sb.ToString();
        }
    }
}
