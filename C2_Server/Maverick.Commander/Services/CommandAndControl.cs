using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Models.History;

#pragma warning disable IDE0290

namespace Maverick.Commander.Services
{
    public class CommandAndControl : ICommandAndControl
    {
        private readonly ILogger<CommandAndControl> _logger;
        public CommandAndControl(ILogger<CommandAndControl> logger)
        {
            _logger = logger;
        }

        private readonly List<InfoClientModel> _clients = [];
        private readonly Dictionary<int, CommandRequestModel> _commands = [];
        private readonly Dictionary<int, List<HistoryModel>> _histories = [];

        public List<InfoClientModel> GetAllClients()
        {
            return _clients;
        }

        public List<HistoryModel> GetAllHistories(int clientID)
        {
            return _histories[clientID].ToList();
        }

        public void UpdateNewClient(InfoClientModel clientModel)
        {
            _clients.Add(clientModel);
            return;
        }

        public Task UpdateCommand(CommandRequestModel commandRequestModel)
        {
            _histories[commandRequestModel.InternalID].Add(new HistoryModel
            {
                InternalID = commandRequestModel.InternalID,
                ActionDetails = commandRequestModel,
                DateTime = DateTime.UtcNow
            });
            return Task.CompletedTask;
        }

        public CommandRequestModel? GetCommandForClient(int internalID)
        {
            _commands.TryGetValue(internalID, out var command);
            return command;
        }

        public void AddCommandQueue(CommandRequestModel commandRequestModel)
        {
            _commands[commandRequestModel.InternalID] = commandRequestModel;
        }

    }
}
