using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    /* "MVL" - Master Volume Command 
       "00"-"64"	Volume Level 0 – 100 ( In hexadecimal representation)
       "00"-"50"	Volume Level 0 – 80 ( In hexadecimal representation)
       "UP"	sets Volume Level Up
       "DOWN"	sets Volume Level Down
       "UP1"	sets Volume Level Up 1dB Step
       "DOWN1"	sets Volume Level Down 1dB Step
       "QSTN"	gets the Volume Level
     */
    public class MasterVolumeCommand : Command<MasterVolumeCommand>, ICommand
    {
        private const string B = "!1MVL";
        public string Command { get; set; }
        public string Name { get; set; }
        //TODO make it geter
        public int lvl { get; set; }

        public MasterVolumeCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public MasterVolumeCommand(){}

        public override MasterVolumeCommand ParsePacket(string command)
        {
            var m = Regex.Match(command, "([A-Z0-9]{2})");
            if (m.Success)
            {
                MasterVolumeCommand r = SetLvl(int.Parse(command, NumberStyles.HexNumber));
                r.lvl = int.Parse(command, NumberStyles.HexNumber);
                return r;
            }
            return command switch
            {
                "UP" => new MasterVolumeCommand($"{B}UP", "VolumeUp"),
                "UP1" => new MasterVolumeCommand($"{B}UP1", "VolumeUp1"),
                "DOWN" => new MasterVolumeCommand($"{B}DOWN", "VolumeDown"),
                "DOWN1" => new MasterVolumeCommand($"{B}DOWN1", "VolumeDown1"),
                "QSTN" => new MasterVolumeCommand($"{B}QSTN", "Status"),
                _ => new MasterVolumeCommand()
            };
        }

        public byte[] GetBytes() => base.GetBytes(Command);
        
        public override string ToString() =>string.Format("Master Volume: {0}", lvl);

        private MasterVolumeCommand SetLvl(int lvl)
        {
            if (lvl >= 0 && lvl <= 80)
            {
                // var status = Status;
                return new MasterVolumeCommand("!1MVL" + string.Format("{0:X2}", lvl), "SetLevel");
            }

            throw new ArgumentException("Volume-range 0-80 (0x00-0x50 in hex)");
        }
    }
}
