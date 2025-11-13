using System.Text.Json.Serialization;

namespace Maverick.Models.Command
{
    public class CommandRequestModel
    {
        public int InternalID { get; set; }
        public CommandTypes ActionType { get; set; }
        public string? Command { get; set; }
        public byte[]? Data { get; set; }
        [JsonIgnore]
        public bool IsResponded { get; set; }
    }
}
