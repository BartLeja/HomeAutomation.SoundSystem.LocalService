using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public enum MasterVolumeCommandEnum
    {
        Up,
        Up1db,
        Down,
        Down1db,
        Status,
    }

    public enum PowerCommandEnum
    {
        On,
        Standby,
        Status,
    }

    public class CommandGenerator
    {
        public static MasterVolumeCommand MasterVolumeCommandGenerator(MasterVolumeCommandEnum masterVolumeCommandEnum)
        {
            string B = "!1MVL";
            return masterVolumeCommandEnum switch
            {
                MasterVolumeCommandEnum.Up => new MasterVolumeCommand( $"{B}UP", "VolumeUp"),
                MasterVolumeCommandEnum.Up1db => new MasterVolumeCommand($"{B}UP1", "VolumeUp1"),
                MasterVolumeCommandEnum.Down => new MasterVolumeCommand($"{B}DOWN", "VolumeDown"),
                MasterVolumeCommandEnum.Down1db => new MasterVolumeCommand($"{B}DOWN1", "VolumeDown1"),
                MasterVolumeCommandEnum.Status => new MasterVolumeCommand($"{B}QSTN", "Status"),
                _ => new MasterVolumeCommand()
            };
        }

        public static PowerCommand PowerCommandGenerator(PowerCommandEnum powerCommandEnum)
        {
            return powerCommandEnum switch
            {
                PowerCommandEnum.On => new PowerCommand("!1PWR01", "On"),
                PowerCommandEnum.Standby => new PowerCommand("!1PWR00", "Standby"),
                PowerCommandEnum.Status => new PowerCommand("!1PWRQSTN", "Status"),
                _ => new PowerCommand()
            };
        }
    }
}
