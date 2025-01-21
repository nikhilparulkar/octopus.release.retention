using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NFluent;
using NSubstitute;
using OctopusDeploy.Services;
using Xunit;
using NSubstitute.ReceivedExtensions;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace Release.Retention.Tests
{
    public class ReleaseRetentionUnitTests
    {

        [Theory]
        [InlineData(2, new string[] { "Release-1", "Release-2", "Release-6", "Release-7", "Release-8" })]
        [InlineData(1, new string[] { "Release-1", "Release-2", "Release-6", "Release-8" })]
        [InlineData(3, new string[] { "Release-1", "Release-2", "Release-6", "Release-7", "Release-5", "Release-8" })]
        public void TestWhen(int keepCount, string[] expectedRetainedReleases)
        {
            var logger = new NullLogger<IReleaseRetention>();
            var readRels = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Release>>>();
            var readDeps = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Deployment>>>();
            var readPrjs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Project>>>();
            var readEnvs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Environment>>>();
            var sut = new ReleaseRetention(readEnvs, readPrjs,readDeps,readRels, logger);

            readRels.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Release>>("[\r\n  {\r\n    \"Id\": \"Release-1\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-2\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.1\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-3\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": null,\r\n    \"Created\": \"2000-01-02T13:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-4\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-5\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.1-ci1\",\r\n    \"Created\": \"2000-01-01T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-6\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.2\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-7\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.3\",\r\n    \"Created\": \"2000-01-02T12:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-8\",\r\n    \"ProjectId\": \"Project-3\",\r\n    \"Version\": \"2.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  }\r\n]"));

            readDeps.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Deployment>>("[\r\n  {\r\n    \"Id\": \"Deployment-1\",\r\n    \"ReleaseId\": \"Release-1\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-01T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-2\",\r\n    \"ReleaseId\": \"Release-2\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-02T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-3\",\r\n    \"ReleaseId\": \"Release-1\",\r\n    \"EnvironmentId\": \"Environment-2\",\r\n    \"DeployedAt\": \"2000-01-02T11:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-4\",\r\n    \"ReleaseId\": \"Release-2\",\r\n    \"EnvironmentId\": \"Environment-3\",\r\n    \"DeployedAt\": \"2000-01-02T12:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-5\",\r\n    \"ReleaseId\": \"Release-5\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-01T11:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-6\",\r\n    \"ReleaseId\": \"Release-6\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-02T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-7\",\r\n    \"ReleaseId\": \"Release-6\",\r\n    \"EnvironmentId\": \"Environment-2\",\r\n    \"DeployedAt\": \"2000-01-02T11:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-8\",\r\n    \"ReleaseId\": \"Release-7\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-02T13:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-9\",\r\n    \"ReleaseId\": \"Release-6\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-02T14:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Deployment-10\",\r\n    \"ReleaseId\": \"Release-8\",\r\n    \"EnvironmentId\": \"Environment-1\",\r\n    \"DeployedAt\": \"2000-01-01T10:00:00\"\r\n  }\r\n]"));
            readPrjs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Project>>("[\r\n  {\r\n    \"Id\": \"Project-1\",\r\n    \"Name\": \"Random Quotes\"\r\n  },\r\n  {\r\n    \"Id\": \"Project-2\",\r\n    \"Name\": \"Pet Shop\"\r\n  }\r\n]"));
            readEnvs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Environment>>("[\r\n  {\r\n    \"Id\": \"Environment-1\",\r\n    \"Name\": \"Staging\"\r\n  },\r\n  {\r\n    \"Id\": \"Environment-2\",\r\n    \"Name\": \"Production\"\r\n  }\r\n]"));

            var result = sut.RetainReleases(keepCount,"path");
            Check.That(result).IsNotNull();
            Check.That(result).Contains(expectedRetainedReleases);
         }

        [Fact]
        public void TestWhenAnyOfJsonFileIsEmpty()
        {
            var fileReader = Substitute.For<IFileReader>();
            var logger = new NullLogger<IReleaseRetention>();
            var readRels = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Release>>>();
            var readDeps = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Deployment>>>();
            var readPrjs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Project>>>();
            var readEnvs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Environment>>>();
            var sut = new ReleaseRetention(readEnvs, readPrjs, readDeps, readRels, logger);

            readRels.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Release>>("[\r\n  {\r\n    \"Id\": \"Release-1\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-2\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.1\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-3\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": null,\r\n    \"Created\": \"2000-01-02T13:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-4\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-5\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.1-ci1\",\r\n    \"Created\": \"2000-01-01T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-6\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.2\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-7\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.3\",\r\n    \"Created\": \"2000-01-02T12:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-8\",\r\n    \"ProjectId\": \"Project-3\",\r\n    \"Version\": \"2.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  }\r\n]"));

            readDeps.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Deployment>>(""));
            readPrjs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Project>>("[\r\n  {\r\n    \"Id\": \"Project-1\",\r\n    \"Name\": \"Random Quotes\"\r\n  },\r\n  {\r\n    \"Id\": \"Project-2\",\r\n    \"Name\": \"Pet Shop\"\r\n  }\r\n]"));
            readEnvs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Environment>>("[\r\n  {\r\n    \"Id\": \"Environment-1\",\r\n    \"Name\": \"Staging\"\r\n  },\r\n  {\r\n    \"Id\": \"Environment-2\",\r\n    \"Name\": \"Production\"\r\n  }\r\n]"));

            var result = sut.RetainReleases(2, "path");
            Check.That(result).IsEmpty();
         }

        [Fact]
        public void TestWhenExceptionOccurs()
        {
            var logger = new NullLogger<IReleaseRetention>();
            var readRels = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Release>>>();
            var readDeps = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Deployment>>>();
            var readPrjs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Project>>>();
            var readEnvs = Substitute.For<IJsonReader<List<OctopusDeploy.Models.Environment>>>();
            var sut = new ReleaseRetention(readEnvs, readPrjs, readDeps, readRels, logger);

            readRels.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Release>>("[\r\n  {\r\n    \"Id\": \"Release-1\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-2\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": \"1.0.1\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-3\",\r\n    \"ProjectId\": \"Project-1\",\r\n    \"Version\": null,\r\n    \"Created\": \"2000-01-02T13:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-4\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-5\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.1-ci1\",\r\n    \"Created\": \"2000-01-01T10:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-6\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.2\",\r\n    \"Created\": \"2000-01-02T09:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-7\",\r\n    \"ProjectId\": \"Project-2\",\r\n    \"Version\": \"1.0.3\",\r\n    \"Created\": \"2000-01-02T12:00:00\"\r\n  },\r\n  {\r\n    \"Id\": \"Release-8\",\r\n    \"ProjectId\": \"Project-3\",\r\n    \"Version\": \"2.0.0\",\r\n    \"Created\": \"2000-01-01T09:00:00\"\r\n  }\r\n]"));

            readDeps.ReadJson(Arg.Any<String>()).Throws(new ArgumentNullException());
            readPrjs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Project>>("[\r\n  {\r\n    \"Id\": \"Project-1\",\r\n    \"Name\": \"Random Quotes\"\r\n  },\r\n  {\r\n    \"Id\": \"Project-2\",\r\n    \"Name\": \"Pet Shop\"\r\n  }\r\n]"));
            readEnvs.ReadJson(Arg.Any<String>()).Returns(JsonConvert.DeserializeObject<List<OctopusDeploy.Models.Environment>>("[\r\n  {\r\n    \"Id\": \"Environment-1\",\r\n    \"Name\": \"Staging\"\r\n  },\r\n  {\r\n    \"Id\": \"Environment-2\",\r\n    \"Name\": \"Production\"\r\n  }\r\n]"));

            var result = sut.RetainReleases(2, "path");
            Check.That(result).IsEmpty();

        }
    }
}
