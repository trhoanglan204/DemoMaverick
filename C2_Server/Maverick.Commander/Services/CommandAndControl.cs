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

        private static readonly List<InfoClientModel> _clients = [];
        private static readonly Dictionary<int, CommandRequestModel> _commands = [];
        private static readonly Dictionary<int, List<HistoryModel>> _histories = [];
        private static readonly HashSet<int> ListClientId = [];

        public int GenerateNewClientID()
        {
            var rand = new Random();
            int newID;
            do
            {
                newID = rand.Next(1000, 9999);
            } while (ListClientId.Contains(newID));
            return newID;
        }   

        public bool AddClientID(int clientID)
        {
            return ListClientId.Add(clientID);
        }

        public bool RemoveClientID(int clientID)
        {
            return ListClientId.Remove(clientID);
        }

        public bool ValidListClientID(int clientID)
        {
            return ListClientId.Contains(clientID);
        }

        public List<InfoClientModel>? GetAllClients()
        {
            return _clients;
        }

        public List<HistoryModel>? GetAllHistories(int clientID)
        {
            _histories.TryGetValue(clientID, out var history);
            return history;
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
