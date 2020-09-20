using System;
using System.Collections.Generic;
using System.Text;
using static HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services.DeviceDiscoveryService;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface IDeviceDiscoveryService
    {
        DiscoveryResult DiscoverDevice(int port);
    }
}
