using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface IAddressResolutionProtocolService
    {
        DeviceAddressInfo GetIPInfo(string macAddress);

        Task<DeviceAddressInfo> GetIPInfo(string macAddress, IEnumerable<string> ips);
    }
}
