using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi
{
    public interface ISoundControllerApi
    {
        void StartSoundApi(string onkyoUrl);
        Task MasterVolumeUp();
        void MasterVolumeDown();
        void PowerOn();
        void PowerOff();
        void MasterVolumeStatus();
        event PacketRecievedApi OnPacketRecieved;
    }
}
