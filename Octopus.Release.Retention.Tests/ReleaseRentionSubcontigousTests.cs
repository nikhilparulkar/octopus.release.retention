using Microsoft.Extensions.Logging;
using NFluent;
using NSubstitute;
using OctopusDeploy.Services;
using Xunit;

namespace Octopus.Release.Retention.Tests
{
    public class ReleaseRentionSubcontigousTests
    {
        [Theory]
        [InlineData(2, "/Inputs/TestSet1")]
        [InlineData(3, "/Inputs/TestSet2")]
        [InlineData(0, "/Inputs/TestSet3")]
        [InlineData(2, "/Inputs/TestSet4")]
        [InlineData(2, "/Inputs/TestSet5")]

        public void TestWithVariousTestDataSets(int keepCount, string pathToTestData)
        {
            var logger = Substitute.For<ILogger<IReleaseRetention>>();
            
            var filereader = new FileReader();
            var readRels = new JsonReader<List<OctopusDeploy.Models.Release>>(filereader);
            var readDeps = new JsonReader<List<OctopusDeploy.Models.Deployment>>(filereader);
            var readPrjs = new JsonReader<List<OctopusDeploy.Models.Project>>(filereader);
            var readEnvs = new JsonReader<List<OctopusDeploy.Models.Environment>>(filereader);
            var sut = new ReleaseRetention(readEnvs, readPrjs, readDeps, readRels, logger);

            var resultReader = new JsonReader<List<string>>( filereader);

            var expected = resultReader.ReadJson($"{System.IO.Directory.GetCurrentDirectory()}{pathToTestData}/ExpectedResult.json");
            
            var result = sut.RetainReleases(keepCount, pathToTestData);

            Check.That(result).Contains(expected);            
        }
    }
}
