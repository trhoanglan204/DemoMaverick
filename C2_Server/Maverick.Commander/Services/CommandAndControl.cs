using Maverick.Models;
using Maverick.Models.Command;
using Maverick.Models.History;
using System.ComponentModel.Design;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        private static readonly Dictionary<int, InfoClientModel> _clients = [];
        private static readonly Dictionary<int, Queue<CommandRequestModel>> _commands = [];
        private static readonly Dictionary<int, List<HistoryModel>> _histories = [];
        private static readonly Dictionary<int, int> _commandToClient = [];

        private readonly Random _rng = new();

        public int GenerateNewClientID()
        {
            int newID;
            do
            {
                newID = _rng.Next(1000, 9999);
            } while (_clients.ContainsKey(newID));
            return newID;
        }  

        public bool ValidListClientID(int clientID)
        {
            return _clients.ContainsKey(clientID);
        }

        public List<InfoClientModel>? GetAllClients()
        {
            return [.. _clients.Values];
        }

        public bool RemoveClient(int internalID)
        {
            return _clients.Remove(internalID);
        }

        public List<HistoryModel>? GetAllHistories(int clientID)
        {
            _histories.TryGetValue(clientID, out var history);
            return history;
        }

        public void UpdateNewClient(InfoClientModel clientModel)
        {
            _clients[clientModel.InternalID] = clientModel;
            if (!_histories.ContainsKey(clientModel.InternalID))
                _histories[clientModel.InternalID] = [];
            return;
        }

        public Task UpdateCommand(CommandRequestModel commandRequestModel)
        {
            if (_histories.TryGetValue(commandRequestModel.InternalID, out var list))
            {
                var latest = list.LastOrDefault(x => x.HistoryID == commandRequestModel.CommandID);
                if (latest != null)
                {
                    latest.ActionDetails = commandRequestModel;
                    latest.Status = CommandStatus.Responded;
                    latest.DateTime = DateTime.UtcNow;
                }
            }
            return Task.CompletedTask;
        }

        public CommandRequestModel? GetCommandForClient(int internalID)
        {
            if (!_commands.TryGetValue(internalID, out var queue))
                return null;
            if (queue.Count == 0)
                return null;
            var cmd = queue.Peek(); 
            MarkAsReceived(cmd);
            return queue.Dequeue();
        }

        public void AddCommandQueue(CommandRequestModel cmd)
        {
            if (!_commands.ContainsKey(cmd.InternalID))
                _commands[cmd.InternalID] = new Queue<CommandRequestModel>(); //init

            if (!_histories.ContainsKey(cmd.InternalID))
                _histories[cmd.InternalID] = [];

            _commands[cmd.InternalID].Enqueue(cmd);

            _commandToClient[cmd.CommandID] = cmd.InternalID;

            _histories[cmd.InternalID].Add(new HistoryModel
            {
                HistoryID = cmd.CommandID,
                InternalID = cmd.InternalID,
                ActionDetails = cmd,
                DateTime = DateTime.UtcNow,
                Status = CommandStatus.Sent
            });
        }

        public void ClearCommand(int commandID)
        {
            if (!_commandToClient.TryGetValue(commandID, out var clientID))
                return;

            if (!_commands.TryGetValue(clientID, out var queue) || queue.Count == 0)
                return;

            var newQueue = new Queue<CommandRequestModel>();
            while (queue.Count > 0)
            {
                var cmd = queue.Dequeue();
                if (cmd.CommandID != commandID)
                    newQueue.Enqueue(cmd);
            }

            _commands[clientID] = newQueue;
            _commandToClient.Remove(commandID);
        }

        private static void MarkAsReceived(CommandRequestModel cmd)
        {
            if (_histories.TryGetValue(cmd.InternalID, out var list))
            {
                var latest = list.LastOrDefault(x => x.ActionDetails.CommandID == cmd.CommandID);
                if (latest != null)
                    latest.Status = CommandStatus.Received;
            }
        }

    }
}
