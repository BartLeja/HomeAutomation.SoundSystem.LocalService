using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface ISocketService
    {
        string DeviceIp { get; set; }
        int DevicePort { get; set; }
        //bool blocked { get; set; }

        //delegate void PacketRecieved(string str);
        event PacketRecieved OnPacketRecieved;
        void SendPacket(ICommand packet, bool blocking = false);
        void StartListener();
        void StopListener();
        void Dispose();
    }
}
