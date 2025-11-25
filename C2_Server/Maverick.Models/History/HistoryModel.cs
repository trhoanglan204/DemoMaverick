using Maverick.Models.Command;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maverick.Models.History
{
    public class HistoryModel
    {
        public int HistoryID { get; set; }
        public int InternalID { get; set; }
        public CommandRequestModel? ActionDetails { get; set; }
        public CommandStatus Status { get; set; }
        public DateTime DateTime { get; set; }
    }

    public enum CommandStatus
    {
        Sent,        
        Received,
        Responded
    }
}
