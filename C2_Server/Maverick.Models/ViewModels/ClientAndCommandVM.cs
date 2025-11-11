using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Models.ViewModels
{
    public class ClientAndCommandVM
    {
        public List<InfoClientModel>? Clients { get; set; }
        public List<HistoryModel>? Histories { get; set; }
    }
}
