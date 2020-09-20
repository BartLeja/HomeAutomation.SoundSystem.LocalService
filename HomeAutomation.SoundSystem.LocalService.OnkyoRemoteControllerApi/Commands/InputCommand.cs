using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public class InputCommand : Command<InputCommand>, ICommand
    {
        /*"SLI" - Input Selector Command 	
      "00"	sets VIDEO1    VCR/DVR
      "01"	sets VIDEO2    CBL/SAT
      "02"	sets VIDEO3    GAME/TV    GAME
      "03"	sets VIDEO4    AUX1(AUX)
      "04"	sets VIDEO5    AUX2
      "05"	sets VIDEO6    PC
      "06"	sets VIDEO7
      "07"	Hidden1
      "08"	Hidden2
      "09"	Hidden3
      "10"	sets DVD          BD/DVD
      "20"	sets TAPE(1)    TV/TAPE
      "21"	sets TAPE2
      "22"	sets PHONO
      "23"	sets CD    TV/CD
      "24"	sets FM
      "25"	sets AM
      “26”	sets TUNER
      "27"	sets MUSIC SERVER    P4S   DLNA*2
      "28"	sets INTERNET RADIO           iRadio Favorite*3
      "29"	sets USB/USB(Front)
      "2A"	sets USB(Rear)
      "2B"	sets NETWORK                      NET
      "2C"	sets USB(toggle)
      "40"	sets Universal PORT
      "30"	sets MULTI CH
      "31"	sets XM*1
      "32"	sets SIRIUS*1
      "UP"	sets Selector Position Wrap-Around Up
      "DOWN"	sets Selector Position Wrap-Around Down
      "QSTN"	gets The Selector Position
      */

        public string Command { get; set; }

        public InputCommand(string command, string displayName)
        {
            Command = command;
            InputName = displayName;
        }

        public InputCommand(string command)
        {
            Command = command;
        }

        public InputCommand()
        {
        }

        [CommandAttribute("!1SLI00")]
        public  InputCommand Video1
        {
            get { return new InputCommand("!1SLI00", "VCR/DVR"); }
        }

        [CommandAttribute("!1SLI01")]
        public  InputCommand Video2
        {
            get { return new InputCommand("!1SLI01", "CBL/SAT"); }
        }

        [CommandAttribute("!1SLI02")]
        public  InputCommand Video3
        {
            get { return new InputCommand("!1SLI02", "GAME"); }
        }

        [CommandAttribute("!1SLI03")]
        public  InputCommand Video4
        {
            get { return new InputCommand("!1SLI03", "AUX1"); }
        }

        [CommandAttribute("!1SLI04")]
        public  InputCommand Video5
        {
            get { return new InputCommand("!1SLI04", "AUX2"); }
        }

        [CommandAttribute("!1SLI05")]
        public  InputCommand Video6
        {
            get { return new InputCommand("!1SLI05", "PC"); }
        }

        [CommandAttribute("!1SLI06")]
        public  InputCommand Video7
        {
            get { return new InputCommand("!1SLI06", "VIDEO 7"); }
        }

        [CommandAttribute("!1SLI07")]
        public  InputCommand Hidden1
        {
            get { return new InputCommand("!1SLI07", "HIDDEN 1"); }
        }

        [CommandAttribute("!1SLI08")]
        public  InputCommand Hidden2
        {
            get { return new InputCommand("!1SLI08", "HIDDEN 2"); }
        }

        [CommandAttribute("!1SLI09")]
        public InputCommand Hidden3
        {
            get { return new InputCommand("!1SLI09", "HIDDEN 3"); }
        }

        [CommandAttribute("!1SLI10")]
        public  InputCommand DVD
        {
            get { return new InputCommand("!1SLI10", "BD/DVD"); }
        }

        [CommandAttribute("!1SLI20")]
        public  InputCommand Tape1
        {
            get { return new InputCommand("!1SLI20", "TV/TAPE(1)"); }
        }

        [CommandAttribute("!1SLI21")]
        public  InputCommand Tape2
        {
            get { return new InputCommand("!1SLI21", "TAPE 2"); }
        }

        [CommandAttribute("!1SLI22")]
        public  InputCommand Phono
        {
            get { return new InputCommand("!1SLI22", "PHONO"); }
        }

        [CommandAttribute("!1SLI23")]
        public  InputCommand CD
        {
            get { return new InputCommand("!1SLI23", "TV/CD"); }
        }

        [CommandAttribute("!1SLI24")]
        public  InputCommand FM
        {
            get { return new InputCommand("!1SLI24", "FM"); }
        }

        [CommandAttribute("!1SLI25")]
        public  InputCommand AM
        {
            get { return new InputCommand("!1SLI25", "AM"); }
        }

        [CommandAttribute("!1SLI26")]
        public  InputCommand Tuner
        {
            get { return new InputCommand("!1SLI26", "TUNER"); }
        }

        [CommandAttribute("!1SLI27")]
        public InputCommand DLNA
        {
            get { return new InputCommand("!1SLI27", "DLNA"); }
        }

        [CommandAttribute("!1SLI28")]
        public  InputCommand InternetRadio
        {
            get { return new InputCommand("!1SLI28", "INTERNET RADIO"); }
        }

        [CommandAttribute("!1SLI29")]
        public  InputCommand FrontUSB
        {
            get { return new InputCommand("!1SLI29", "FRONT USB"); }
        }

        [CommandAttribute("!1SLI2A")]
        public  InputCommand RearUSB
        {
            get { return new InputCommand("!1SLI2A", "REAR USB"); }
        }

        [CommandAttribute("!1SLI2B")]
        public  InputCommand Network
        {
            get { return new InputCommand("!1SLI2B", "NET"); }
        }

        [CommandAttribute("!1SLIUP")]
        public InputCommand Next
        {
            get { return new InputCommand("!1SLIUP", "Next input"); }
        }

        [CommandAttribute("!1SLIDOWN")]
        public InputCommand Previous
        {
            get { return new InputCommand("!1SLIDOWN", "Previous input"); }
        }

        [CommandAttribute("!1SLIQSTN")]
        public  InputCommand Status
        {
            get { return new InputCommand("!1SLIQSTN", "Status"); }
        }

        public string InputName { get; set; }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override InputCommand ParsePacket(string command)
        {
            //Input i = default(Input);
            Type type = typeof(InputCommand); // MyClass is static class with static properties
            var list = from p in type.GetProperties()
                       let v = p.GetCustomAttributes(typeof(CommandAttribute), false)
                       where ((CommandAttribute)v[0]).Name == command
                       select p;
            foreach (PropertyInfo p in list)
            {
                return (InputCommand)p.GetValue(null, null);
            }
            throw new ArgumentException("Cannot parse " + command, "command");
        }

        public byte[] GetBytes()
        {
            return base.GetBytes(Command);
        }

        public override string ToString()
        {
            return string.Format("{0}",
                                 InputName);
        }
    }
}
