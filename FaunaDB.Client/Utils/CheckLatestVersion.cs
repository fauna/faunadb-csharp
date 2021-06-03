using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace FaunaDB.Client.Utils
{
    public class CheckLatestVersion
    {
        [System.AttributeUsage(System.AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
        sealed class ConfigurationLocationAttribute : System.Attribute
        {
            public string ConfigurationLocation { get; }
            public ConfigurationLocationAttribute(string configurationLocation)
            {
                this.ConfigurationLocation = configurationLocation;
            }
        }
        public async Task GetVersionAsync()
        {
            var packageName = "FaunaDB.Client";
            var url = $"https://api.nuget.org/v3-flatcontainer/{packageName}/index.json";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            string versionsResponse = await response.Content.ReadAsStringAsync();
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(versionsResponse);
            var latestVesrion = ((Newtonsoft.Json.Linq.JArray)jObject.First.First).Children().LastOrDefault();
            var currentVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            var fileVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().FullName;
//#if NET20_OR_GREATER
            //string Version = System.Reflection.Assembly.GetEntryAssembly().GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>().InformationalVersion.ToString();

//            var asmbl =  System.AppDomain.CurrentDomain.GetAssemblies();
//#endif
            //var configurationLocation = Assembly.GetEntryAssembly()
            //            .GetCustomAttribute<ConfigurationLocationAttribute>()
            //            .ConfigurationLocation;
        }
    }
}
