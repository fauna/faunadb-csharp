using System.Collections.Generic;
using FaunaDB.Client;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class EnvironmentHeaderTest
    {
        private IEnvironmentEditor environmentEditor;

        [OneTimeSetUp]
        public void SetUp()
        {
            environmentEditor = new EnvironmentEditorMock();
        }

        [SetUp]
        public void DestroyRuntimeEnvironmentHeaderInstance()
        {
            RuntimeEnvironmentHeader.Destroy();
        }

        [Test]
        public void TestRuntimeEnvironmentHeaderFormat()
        {
            string actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Match("driver=csharp-.*; runtime=.*; env=.*; os=.*"));
        }

        [Test]
        public void TestNetlifyEnvironment()
        {
            environmentEditor.SetVariable("NETLIFY_IMAGES_CDN_DOMAIN", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Netlify"));
        }

        [Test]
        public void TestVercelEnvironment()
        {
            environmentEditor.SetVariable("VERCEL", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Vercel"));
        }

        [Test]
        public void TestHerokuEnvironment()
        {
            environmentEditor.SetVariable("PATH", "heroku");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Heroku"));
        }

        [Test]
        public void TestUnknownEnvironmentWithPathVariable()
        {
            environmentEditor.SetVariable("PATH", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Unknown"));
        }

        [Test]
        public void TestAwsLambdaEnvironment()
        {
            environmentEditor.SetVariable("AWS_LAMBDA_FUNCTION_VERSION", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("AWS Lambda"));
        }

        [Test]
        public void TestGoogleFunctionsEnvironment()
        {
            environmentEditor.SetVariable("_", "google");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("GCP Cloud Functions"));
            RuntimeEnvironmentHeader.Destroy();
        }

        [Test]
        public void TestUnknownEnvironmentWithUnderscoreVariable()
        {
            environmentEditor.SetVariable("_", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Unknown"));
        }

        [Test]
        public void TestGoogleCloudEnvironment()
        {
            environmentEditor.SetVariable("GOOGLE_CLOUD_PROJECT", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("GCP Compute Instances"));
        }

        [Test]
        public void TestAzureEnvironment()
        {
            environmentEditor.SetVariable("ORYX_ENV_TYPE", "AppService");
            environmentEditor.SetVariable("WEBSITE_INSTANCE_ID", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Azure Compute"));
        }

        [Test]
        public void TestUnknownEnvironmentWithOryx()
        {
            environmentEditor.SetVariable("ORYX_ENV_TYPE", "some_value");
            environmentEditor.SetVariable("WEBSITE_INSTANCE_ID", "some_value");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Unknown"));
        }

        [Test]
        public void TestUnknownEnvironmentWithOryxWithoutWebsiteInstanceId()
        {
            environmentEditor.SetVariable("ORYX_ENV_TYPE", "AppService");
            var actual = RuntimeEnvironmentHeader.Construct(environmentEditor);
            Assert.That(actual, Does.Contain("Unknown"));
        }

        [TearDown]
        public void RemoveEnvironmentVariables()
        {
            environmentEditor.RemoveVariable("NETLIFY_IMAGES_CDN_DOMAIN");
            environmentEditor.RemoveVariable("VERCEL");
            environmentEditor.RemoveVariable("PATH");
            environmentEditor.RemoveVariable("AWS_LAMBDA_FUNCTION_VERSION");
            environmentEditor.RemoveVariable("_");
            environmentEditor.RemoveVariable("GOOGLE_CLOUD_PROJECT");
            environmentEditor.RemoveVariable("ORYX_ENV_TYPE");
            environmentEditor.RemoveVariable("WEBSITE_INSTANCE_ID");
        }
    }

    internal class EnvironmentEditorMock : IEnvironmentEditor
    {
        private Dictionary<string, string> mockEnvironment;

        public EnvironmentEditorMock()
        {
            mockEnvironment = new Dictionary<string, string>();
        }

        public string GetVariable(string variableName)
        {
            if (!mockEnvironment.ContainsKey(variableName))
            {
                return null;
            }

            return mockEnvironment[variableName];
        }

        public void SetVariable(string variableName, string variableValue)
        {
            mockEnvironment[variableName] = variableValue;
        }

        public void RemoveVariable(string variableName)
        {
            mockEnvironment.Remove(variableName);
        }
    }
}
