namespace Maverick.Models
{
    public class CommandTypesModel
    {
    }

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
        MOUSECLICK,
        KEYBOARDONECHAR,
        KEYBOARDMULTIPLESCHARS,
        GETMODULE,
        LISTALLHANDLESOPENEDS,
        KILLPROCESS,
        CLOSEHANDLE,
    }
}
