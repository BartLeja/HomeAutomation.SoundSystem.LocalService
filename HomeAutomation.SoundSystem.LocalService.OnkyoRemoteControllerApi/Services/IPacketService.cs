using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public interface IPacketService
    {
        byte[] GetBytes();
        void SetCommand(string command);
        ICommand ParsePacket(string packetstring);
    }
}
