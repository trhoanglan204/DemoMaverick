namespace Maverick.Models
{
    public class HistoryModel
    {
        public int InternalID { get; set; }
        public string? AgentID { get; set; }
        public string? ActionType { get; set; }
        public string? ActionDetails { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
