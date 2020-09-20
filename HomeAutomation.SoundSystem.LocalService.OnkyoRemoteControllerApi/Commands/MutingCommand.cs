using System;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public class MutingCommand : Command<MutingCommand>, ICommand
    {
        /*"AMT" - Audio Muting Command	
     "00"	sets Audio Muting Off
     "01"	sets Audio Muting On
     "TG"	sets Audio Muting Wrap-Around
     "QSTN"	gets the Audio Muting State
     */
        public string Name { get; set; }
        public string Command { get; set; }

        public MutingCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public MutingCommand()
        {
        }

        public static MutingCommand On
        {
            get { return new MutingCommand("!1AMT01", "On"); }
        }

        public static MutingCommand Off
        {
            get { return new MutingCommand("!1AMT00", "Off"); }
        }

        public static MutingCommand Toggle
        {
            get { return new MutingCommand("!1AMTTG", "Toggle"); }
        }

        public static MutingCommand Status
        {
            get { return new MutingCommand("!1AMTQSTN", "Status"); }
        }

      
        public override MutingCommand ParsePacket(string command)
        {
            switch (command)
            {
                case "01":
                    return On;
                case "00":
                    return Off;
                case "TG":
                    return Toggle;
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
            return string.Format("Muting: " + Name);
        }
    }
}
