using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Models.History;

namespace Maverick.Commander.Services
{
    public interface ICommandAndControl
    {
        List<InfoClientModel>? GetAllClients();
        public bool RemoveClient(int internalID);

        List<HistoryModel>? GetAllHistories(int clientID);

        void UpdateNewClient(InfoClientModel clientModel);

        Task UpdateCommand(CommandRequestModel commandRequestModel);
        void ClearCommand(int id);

        CommandRequestModel? GetCommandForClient(int internalID);

        void AddCommandQueue(CommandRequestModel commandRequestModel);

        int GenerateNewClientID();
        bool ValidListClientID(int clientID);
    }
}
