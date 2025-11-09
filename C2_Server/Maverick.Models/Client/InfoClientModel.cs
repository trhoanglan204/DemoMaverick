namespace Maverick.Models
{
    public class InfoClientModel
    {
        public int InternalID { get; set; }
        public string? AgentID { get; set; }
        public string? Username { get; set; }
        public string? Hostname { get; set; }
        public string? OSversion { get; set; }
        public string? ClientVersion { get; set; }
        public int NumOfMonitors { get; set; }
        public string? HomePage { get; set; }
    }
}
