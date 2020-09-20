using System;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public class OSDCommand : Command<OSDCommand>, ICommand
    {
        /*"OSD" - Setup Operation Command 	
     "MENU"	Menu Key
     "UP"	Up Key
     "DOWN"	Down Key
     "RIGHT"	Right Key
     "LEFT"	Left Key
     "ENTER"	Enter Key
     "EXIT"	Exit Key
     "AUDIO"	Audio Adjust Key
     "VIDEO"	Video Adjust Key
     "HOME"	Home Key
     */
        public string Command { get; set; }
        public string Name { get; set; }

        public OSDCommand(string command, string name)
        {
            Command = command;
            Name = name;
        }

        public  OSDCommand Menu
        {
            get { return new OSDCommand("!1OSDMENU", "Menu"); }
        }

        public OSDCommand Up
        {
            get { return new OSDCommand("!1OSDUP", "Up"); }
        }

        public OSDCommand Down
        {
            get { return new OSDCommand("!1OSDDOWN", "Down"); }
        }

        public OSDCommand Right
        {
            get { return new OSDCommand("!1OSDRIGHT", "Right"); }
        }

        public OSDCommand Left
        {
            get { return new OSDCommand("!1OSDLEFT", "Left"); }
        }

        public OSDCommand Enter
        {
            get { return new OSDCommand("!1OSDENTER", "Enter"); }
        }

        public OSDCommand Exit
        {
            get { return new OSDCommand("!1OSDEXIT", "Exit"); }
        }

        public OSDCommand Audio
        {
            get { return new OSDCommand("!1OSDAUDIO", "Audio"); }
        }

        public OSDCommand Video
        {
            get { return new OSDCommand("!1OSDVIDEO", "Video"); }
        }

        public OSDCommand Home
        {
            get { return new OSDCommand("!1OSDHOME", "Home"); }
        }

        public override OSDCommand ParsePacket(string command)
        {
            switch (command)
            {
                case "MENU":
                    return Menu;
                case "UP":
                    return Up;
                case "DOWN":
                    return Down;
                case "RIGHT":
                    return Right;
                case "LEFT":
                    return Left;
                case "ENTER":
                    return Enter;
                case "EXIT":
                    return Exit;
                case "AUDIO":
                    return Audio;
                case "VIDEO":
                    return Video;
                case "HOME":
                    return Home;
            }
            throw new ArgumentException("Cannot find the command", "command");
        }

        public byte[] GetBytes()
        {
            return base.GetBytes(Command);
        }

        public override string ToString()
        {
            return string.Format("OSD Command: {0}" + Name);
        }
    }
}
