using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi
{
    public interface ISoundControllerApi
    {
        Task StartSoundApi(string onkyoUrl);
        Task MasterVolumeUp();
        void MasterVolumeDown();
        void PowerOn();
        void PowerOff();
        void MasterVolumeStatus();
        event PacketRecievedApi OnPacketRecieved;
    }
}
