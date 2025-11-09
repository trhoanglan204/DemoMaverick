using System;

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
        SCREENSHOT,
        KEYLOGGER,
        KEYBOARDONECHAR,
        KEYBOARDMULTIPLESCHARS,
        GETMODULE,
        LISTALLHANDLESOPENEDS,
        KILLPROCESS,
        CLOSEHANDLE,
    }
}
