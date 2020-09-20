using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.Clients
{
    public interface ISignalRClient
    {
         Task ConnectToSignalR(string token, string signalRHubUrl);
    }
}
