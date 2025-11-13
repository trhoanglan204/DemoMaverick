using System;
using System.Data;

namespace Maverick.Agent
{
    public enum CommandTypes
    {
        INFOCLIENT,
        DOCOMMAND,
        GETFILE,
        SENDFILE,
        RECONNECT,
        REBOOT,
        KILLAPPLICATION,
    }

    public class InfoClientModel
    {
        public int InternalID { get; set; }
        public string ClientID { get; set; }
        public string Username { get; set; }
        public string Hostname { get; set; }
        public string OSversion { get; set; }
        public string ClientVersion { get; set; }
        public int NumOfMonitors { get; set; }
        public string ClientIP { get; set; }
    }

    public class CommandRequestModel
    {
        public int InternalID { get; set; }
        public CommandTypes ActionType { get; set; }
        public string Command { get; set; }
        public byte[] Data { get; set; }
    }

    public class FileUploadModel
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }

}
