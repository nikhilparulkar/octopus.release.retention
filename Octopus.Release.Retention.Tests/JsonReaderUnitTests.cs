using OctopusDeploy.Services;
using NFluent;
using NSubstitute;
using Newtonsoft.Json;
using Xunit;

namespace Release.Retention.Tests
{
    public class JsonReaderUnitTests
    {
        [Fact]
        public void Test1()
        {
            var fileReader = Substitute.For<IFileReader>();
           
            var sut = new JsonReader<List<OctopusDeploy.Models.Release>>(fileReader);

            var result = sut.ReadJson("");

            Check.That(result).IsNull();
        }

        [Fact]
        public void Test2()
        {
            var fileReader = Substitute.For<IFileReader>();
            var sut = new JsonReader<List<OctopusDeploy.Models.Release>>(fileReader);


            fileReader.GetFileContent(Arg.Any<String>()).ReturnsForAnyArgs(JsonConvert.SerializeObject(new []{  new
            {
                id = "1",
                ProjectId = "2",
                Version = "ver1",
                Created = "date"

            }}));
            var result = sut.ReadJson("abc.json");

            Check.That(result).IsNotNull();
        }
    }
}