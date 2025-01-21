namespace OctopusDeploy.Services
{
    public class FileReader : IFileReader
    {
        public string GetFileContent(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            using (StreamReader r = new StreamReader(path))
            {
                return r.ReadToEnd();
            }
        }

        
    }
}
