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
        public string? ClientId { get; set; }
        public string? Command { get; set; }
        public DateTime DateTime { get; set; }
    }
}
