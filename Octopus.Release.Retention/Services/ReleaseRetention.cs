using Microsoft.Extensions.Logging;
using OctopusDeploy.Models;

namespace OctopusDeploy.Services
{
    public class ReleaseRetention : IReleaseRetention
    {
        readonly IJsonReader<List<Models.Environment>> _jsonReaderEnv;
        readonly IJsonReader<List<Project>> _jsonReaderPrj;
        readonly IJsonReader<List<Deployment>> _jsonReaderDpmt;
        readonly IJsonReader<List<Release>> _jsonReaderRel;
        readonly ILogger<IReleaseRetention> _logger;

        public ReleaseRetention(IJsonReader<List<Models.Environment>> jsonReaderEnv,
            IJsonReader<List<Project>> jsonReaderPrj,
            IJsonReader<List<Deployment>> jsonReaderDpmt,
            IJsonReader<List<Release>> jsonReaderRel,
            ILogger<IReleaseRetention> logger
            )
        {
            _jsonReaderEnv = jsonReaderEnv;
            _jsonReaderDpmt = jsonReaderDpmt;
            _jsonReaderPrj = jsonReaderPrj;
            _jsonReaderRel = jsonReaderRel;
            _logger = logger;
        }


        public List<string> RetainReleases(int keepCount, string pathToInputFiles)
        {
            var path = $"{System.IO.Directory.GetCurrentDirectory()}{pathToInputFiles}";
            try
            {
                //Read and Validate Inputs 
                var envList = _jsonReaderEnv.ReadJson($"{path}/Environments.json");
                var prjList = _jsonReaderPrj.ReadJson($"{path}/Projects.json");
                var dmptList = _jsonReaderDpmt.ReadJson($"{path}/Deployments.json");
                var relList = _jsonReaderRel.ReadJson($"{path}/Releases.json");

                if (!envList.Any() || !prjList.Any() || !dmptList.Any() || !relList.Any() )
                {
                    _logger.LogError(" Empty input files ");
                    return new List<string>();
                }
                if (keepCount < 1 || string.IsNullOrWhiteSpace(pathToInputFiles))
                {
                    _logger.LogError(" Invalid Rentention Keep Count, or Invalid Path to Deployment Data ");
                    return new List<string>();
                }

                return ApplyReleaseRetention(relList, dmptList,keepCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<string>();
            }
        }

        private List<string> ApplyReleaseRetention(List<Release> relList, List<Deployment> dmptList, int keepCount)
        {
            var result = new ResultSet();
            
            //Iterate through the deployments and keep the most latest deployment of a release
            foreach (var dep in dmptList)
            {
                var element = result.Items
                    .Where(item => string.Equals(item.ReleaseId, dep.ReleaseId, StringComparison.Ordinal) 
                    && string.Equals(item.EnvironmentId, dep.EnvironmentId, StringComparison.Ordinal))
                    .FirstOrDefault();
                if (element != null)
                {
                    if (DateTime.Compare(element.DeployedAt, dep.DeployedAt) < 0)
                        element.DeployedAt = dep.DeployedAt;
                }
                else
                {
                    result.Items.Add(new SetDetails
                    {
                        ReleaseId = dep.ReleaseId,
                        EnvironmentId = dep.EnvironmentId,
                        DeployedAt = dep.DeployedAt,
                        ProjectId = relList.FirstOrDefault(rel => string.Equals(rel.Id, dep.ReleaseId, StringComparison.Ordinal)).ProjectId
                    });
                }
            }

             
            var res = result.Items.GroupBy(item => new { item.ProjectId, item.EnvironmentId }).ToList();
            var retainedReleases = new List<string>();
            foreach (var grp in res)
            {
                var relIdlist = grp.OrderByDescending(item => item.DeployedAt).Select(x => x.ReleaseId).ToList().Take(keepCount);
                _logger.LogInformation($" For {grp.Key.ProjectId} And {grp.Key.EnvironmentId} Keeping latest {keepCount} releases - \n \t {string.Join("\n \t", relIdlist)}");

                retainedReleases.AddRange(relIdlist); retainedReleases.AddRange(relIdlist);
            }

            // return list of release Id to be retained.
            return retainedReleases.Distinct().ToList();
        }
    }
}
