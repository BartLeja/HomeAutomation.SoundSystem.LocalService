namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    /*"PWR" - System Power Command  	
    "00"	sets System Standby
    "01"	sets System On
    "QSTN"	gets the System Power Status
    */
    public class PowerCommand : Command<PowerCommand>, ICommand
    {
        public string Command { get; set; }
        public string Name { get; set; }

        public PowerCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public PowerCommand(){}

        public override PowerCommand ParsePacket(string command)
        {
            return command switch
            {
                "00" => new PowerCommand("!1PWR00", "Standby"),
                "01" => new PowerCommand("!1PWR01", "On"),
                "QSTN" => new PowerCommand("!1PWRQSTN", "Status"),
                _ => new PowerCommand()
            };
        }

        public byte[] GetBytes() => base.GetBytes(Command);
       
        public override string ToString() => string.Format("System power is {0}", Name);
    }
}
