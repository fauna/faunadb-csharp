using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FaunaDB.Client
{
    internal class RuntimeEnvironmentHeader
    {
        private string runtime;
        private string driverVersion;
        private string operatingSystem;
        private string environment;

        private static RuntimeEnvironmentHeader instance;

        private RuntimeEnvironmentHeader() {}

        private void GatherEnvironmentInfo(IEnvironmentEditor environmentEditor)
        {
            this.environment = GetRuntimeEnvironment(environmentEditor);
            this.operatingSystem = GetOperatingSystemName();
            this.runtime = GetCurrentRuntime();
            this.driverVersion = Assembly.Load(new AssemblyName("FaunaDB.Client")).GetName().Version.ToString();
        }

        private static string GetOperatingSystemName()
        {
#if (NETSTANDARD2_1 || NETSTANDARD2_0 || NETSTANDARD1_5)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            return "Unknown";
#else
            // if we're under one of the .net frameworks than OS is Windows
            return "Windows";
#endif
        }

        private static string GetCurrentRuntime()
        {
#if (NETSTANDARD2_1 || NETSTANDARD2_0 || NETSTANDARD1_5)
            return RuntimeInformation.FrameworkDescription;
#elif NET45
            var versionInfo = typeof(object).Assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute));
            if (versionInfo != null)
            {
                return ((AssemblyInformationalVersionAttribute) versionInfo).InformationalVersion;
            }
            return "unknown .net runtime";
#else
            return "unknown .net runtime";
#endif
        }

        private static string GetRuntimeEnvironment(IEnvironmentEditor environmentEditor)
        {
            var envNetlify = environmentEditor.GetVariable("NETLIFY_IMAGES_CDN_DOMAIN");
            if (envNetlify != null)
            {
                return "Netlify";
            }

            var envVercel = environmentEditor.GetVariable("VERCEL");
            if (envVercel != null)
            {
                return "Vercel";
            }

            var envPath = environmentEditor.GetVariable("PATH");
            if (envPath != null && envPath.Contains("heroku"))
            {
                return "Heroku";
            }

            var envAwsLambda = environmentEditor.GetVariable("AWS_LAMBDA_FUNCTION_VERSION");
            if (envAwsLambda != null)
            {
                return "AWS Lambda";
            }

            var envGcpFunctions = environmentEditor.GetVariable("_");
            if (envGcpFunctions != null && envGcpFunctions.Contains("google"))
            {
                return "GCP Cloud Functions";
            }

            var envGoogleCloud = environmentEditor.GetVariable("GOOGLE_CLOUD_PROJECT");
            if (envGoogleCloud != null)
            {
                return "GCP Compute Instances";
            }

            var envOryx = environmentEditor.GetVariable("ORYX_ENV_TYPE");
            var envWebsiteInstance = environmentEditor.GetVariable("WEBSITE_INSTANCE_ID");
            if (envOryx != null && envWebsiteInstance != null && envOryx.Contains("AppService"))
            {
                return "Azure Compute";
            }

            return "Unknown";
        }

        public static string Construct(IEnvironmentEditor environmentEditor)
        {
            if (instance == null)
            {
                instance = new RuntimeEnvironmentHeader();
                instance.GatherEnvironmentInfo(environmentEditor);
            }

            return
                $"driver=csharp-{instance.driverVersion}; runtime={instance.runtime}; env={instance.environment}; os={instance.operatingSystem}";
        }

        public static void Destroy()
        {
            instance = null;
        }
    }

    internal interface IEnvironmentEditor
    {
        string GetVariable(string variableName);
        void SetVariable(string variableName, string variableValue);
        void RemoveVariable(string variableName);
    }

    internal class EnvironmentEditor : IEnvironmentEditor
    {
        private static EnvironmentEditor instance;

        private EnvironmentEditor() {}

        public static EnvironmentEditor Create()
        {
            if (instance == null)
            {
                instance = new EnvironmentEditor();
            }

            return instance;
        }
        public string GetVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }

        public void SetVariable(string variableName, string variableValue)
        {
            Environment.SetEnvironmentVariable(variableName, variableValue);
        }

        public void RemoveVariable(string variableName)
        {
            Environment.SetEnvironmentVariable(variableName, null);
        }
    }
}
