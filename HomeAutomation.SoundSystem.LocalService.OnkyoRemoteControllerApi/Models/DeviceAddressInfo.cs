namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Models
{
    public class DeviceAddressInfo
    {
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string MacAddress { get; set; }

        public DeviceAddressInfo(string macAddress, string ipAddress)
        {
            IpAddress = ipAddress;
            MacAddress = macAddress;
        }

    }
}
