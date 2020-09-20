using System;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public class PowerCommand : Command<PowerCommand>, ICommand
    {
        /*"PWR" - System Power Command  	
      "00"	sets System Standby
      "01"	sets System On
      "QSTN"	gets the System Power Status
      */
        public string Command { get; set; }
        public string Name { get; set; }

        public PowerCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public PowerCommand()
        {
        }

        public PowerCommand On
        {
            get { return new PowerCommand("!1PWR01", "On"); }
        }

        public PowerCommand Off
        {
            get { return new PowerCommand("!1PWR00", "Standby"); }
        }

        public PowerCommand Status
        {
            get { return new PowerCommand("!1PWRQSTN", "Status"); }
        }

        public event EventHandler CanExecuteChanged;

        public override PowerCommand ParsePacket(string command)
        {
            switch (command)
            {
                case "00":
                    return Off;
                case "01":
                    return On;
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
            return string.Format("System power is {0}", Name);
        }

      
    }
}
