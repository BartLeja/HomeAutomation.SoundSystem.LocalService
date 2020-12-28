namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi
{
    public interface ISoundControllerApi
    {
        void StartSoundApi(string onkyoUrl);
        void MasterVolumeUp();
        void MasterVolumeDown();
        void PowerOn();
        void PowerOff();
        void MasterVolumeStatus();
        event PacketRecievedApi OnPacketRecieved;
    }
}
