using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Models.Command
{
    public class CommandRequestModel
    {
        public int InternalID { get; set; }
        public CommandTypes ActionType { get; set; }
        public string? Command { get; set; }
        public byte[]? Data { get; set; }
    }
}
