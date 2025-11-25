using Maverick.Models.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Models.ViewModels
{
    public class ClientChatVM
    {
        public List<InfoClientModel>? Clients { get; set; }
        public List<HistoryModel>? Histories { get; set; }
        public string? SelectedClientId { get; set; }
    }
}
