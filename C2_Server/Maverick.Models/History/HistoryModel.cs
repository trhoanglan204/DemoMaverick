namespace Maverick.Models
{
    public class HistoryModel
    {
        public int InternalID { get; set; }
        public string? ClientID { get; set; }
        public string? ActionType { get; set; }
        public string? ActionDetails { get; set; }
        public string? ActionResult { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
