namespace OctopusDeploy.Models
{
    public class ResultSet
    {
        public List<SetDetails> Items = new List<SetDetails>();
    }

    public class SetDetails
    {
        public string ReleaseId { get; set; }
        public string EnvironmentId { get; set; }
        public DateTime DeployedAt { get; set; }
        public string ProjectId { get; set; }
    }

}
