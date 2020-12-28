using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public class MasterVolumeCommand : Command<MasterVolumeCommand>, ICommand
    {
        private const string B = "!1MVL";
        /* "MVL" - Master Volume Command 
          "00"-"64"	Volume Level 0 – 100 ( In hexadecimal representation)
          "00"-"50"	Volume Level 0 – 80 ( In hexadecimal representation)
          "UP"	sets Volume Level Up
          "DOWN"	sets Volume Level Down
          "UP1"	sets Volume Level Up 1dB Step
          "DOWN1"	sets Volume Level Down 1dB Step
          "QSTN"	gets the Volume Level
        */
        public string Command { get; set; }
        public string Name { get; set; }

        public MasterVolumeCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public MasterVolumeCommand()
        {
        }

        public MasterVolumeCommand Up
        {
            get { return new MasterVolumeCommand(B + "UP","VolumeUp"); }
        }

        public  MasterVolumeCommand Up1db
        {
            get { return new MasterVolumeCommand(B + "UP1", "VolumeUp1"); }
        }

        public  MasterVolumeCommand Down
        {
            get { return new MasterVolumeCommand(B + "DOWN", "VolumeDown"); }
        }

        public  MasterVolumeCommand Down1db
        {
            get { return new MasterVolumeCommand(B + "DOWN1", "VolumeDown1"); }
        }

        public  MasterVolumeCommand Status
        {
            get { return new MasterVolumeCommand(B + "QSTN","Status"); }
        }

        //TODO make it geter
        public int lvl { get; set; }


        public MasterVolumeCommand SetLvl(int lvl)
        {
            if (lvl >= 0 && lvl <= 80)
            {
                var status = Status;
                return new MasterVolumeCommand("!1MVL" + string.Format("{0:X2}", lvl), "SetLevel");
            }
               
            throw new ArgumentException("Volume-range 0-80 (0x00-0x50 in hex)");
        }

        public override MasterVolumeCommand ParsePacket(string command)
        {
            Match m = Regex.Match(command, "([A-Z0-9]{2})");
            if (m.Success)
            {
                MasterVolumeCommand r = SetLvl(int.Parse(command, NumberStyles.HexNumber));
                r.lvl = int.Parse(command, NumberStyles.HexNumber);
                return r;
            }
            switch (command)
            {
                case "UP":
                    return Up;
                case "UP1":
                    return Up1db;
                case "DOWN":
                    return Down;
                case "DOWN1":
                    return Down1db;
                case "QSTN":
                    return Status;
            }
            throw new ArgumentException("Cannot find the command", "command");
        }

        public byte[] GetBytes()
        {
            return base.GetBytes(Command);
        }

        public override string ToString()
        {
            return string.Format("Master Volume: {0}", lvl);
        }
    }
}
