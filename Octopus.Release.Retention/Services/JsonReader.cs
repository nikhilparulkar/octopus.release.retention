

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OctopusDeploy.Services
{
    public class JsonReader <T> : IJsonReader<T>
    {
        private readonly IFileReader _fileReader;

        public JsonReader(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }
        public T? ReadJson(string filepath)
        {
            var json = _fileReader.GetFileContent(filepath);
               
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
