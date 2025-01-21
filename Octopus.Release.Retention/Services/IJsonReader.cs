namespace OctopusDeploy.Services
{
    public  interface IJsonReader<T>
    {
        public T? ReadJson(string filepath);
    }
}
