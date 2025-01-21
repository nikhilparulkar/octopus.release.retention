
namespace OctopusDeploy.Services
{
    public interface IReleaseRetention
    {
        List<string> RetainReleases(int keepCount, string pathToInputFiles);
    }
}
