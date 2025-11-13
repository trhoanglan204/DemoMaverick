using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Models.History;

namespace Maverick.Commander.Services
{
    public interface ICommandAndControl
    {
        List<InfoClientModel>? GetAllClients();
        List<HistoryModel>? GetAllHistories(int clientID);

        void UpdateNewClient(InfoClientModel clientModel);

        Task UpdateCommand(CommandRequestModel commandRequestModel);

        CommandRequestModel? GetCommandForClient(int internalID);

        void AddCommandQueue(CommandRequestModel commandRequestModel);

        bool RemoveClientID(int clientID);
        bool AddClientID(int clientID);
        int GenerateNewClientID();
        bool ValidListClientID(int clientID);
    }
}
