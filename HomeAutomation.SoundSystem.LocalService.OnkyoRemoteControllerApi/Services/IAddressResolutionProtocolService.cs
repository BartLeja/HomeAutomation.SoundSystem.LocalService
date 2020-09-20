using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface IAddressResolutionProtocolService
    {
        DeviceAddressInfo GetIPInfo(string macAddress);
    }
}
