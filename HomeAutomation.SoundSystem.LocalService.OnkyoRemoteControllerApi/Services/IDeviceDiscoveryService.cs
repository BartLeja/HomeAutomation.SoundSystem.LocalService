using System.Threading.Tasks;
using static HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services.DeviceDiscoveryService;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface IDeviceDiscoveryService
    {
        Task<DiscoveryResult> DiscoverDevice(int port);
    }
}
