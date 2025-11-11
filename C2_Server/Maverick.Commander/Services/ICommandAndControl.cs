using Maverick.Models;
using Maverick.Models.Command;

namespace Maverick.Commander.Services
{
    public interface ICommandAndControl
    {
        List<InfoClientModel> GetAllClients();

        void UpdateNewClient(InfoClientModel clientModel);

        Task UpdateCommand(CommandRequestModel commandRequestModel);

        CommandRequestModel? GetCommandForClient(int internalID);
    }
}
