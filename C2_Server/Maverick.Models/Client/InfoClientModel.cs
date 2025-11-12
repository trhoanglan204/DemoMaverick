using Maverick.Models.History;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Maverick.Models
{
    public class InfoClientModel
    {
        [Key]
        public int InternalID { get; set; }
        public string? ClientID { get; set; }
        public string? Username { get; set; }
        public string? Hostname { get; set; }
        public string? OSversion { get; set; }
        public string? ClientVersion { get; set; }
        public int NumOfMonitors { get; set; }
        public string? ClientIP { get; set; }
    }
}
