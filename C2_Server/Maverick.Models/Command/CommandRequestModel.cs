using Maverick.Models.File;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Maverick.Models.Command
{
    public class CommandRequestModel
    {
        public int InternalID { get; set; }
        public int CommandID { get; set; } = Random.Shared.Next();
        public string? ActionType { get; set; }
        public string? Command { get; set; }
        public byte[]? Data { get; set; }
        public FileUploadModel? FileUpload { get; set; }
    }
}
