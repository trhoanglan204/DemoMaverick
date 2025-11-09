using Maverick.Models;

namespace Maverick.Commander.Services
{
    public interface ICommandAndControl
    {
        List<InfoClientModel> GetAllClients();
    }
}
