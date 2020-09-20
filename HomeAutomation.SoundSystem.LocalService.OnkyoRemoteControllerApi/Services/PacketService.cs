using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public class PacketService : IPacketService
    {
        private const byte EOF = 0x0D;

        private static readonly byte[] PacketTemplate = new byte[]
          {
        0x49, 0x53, 0x43, 0x50,
        0x00, 0x00, 0x00, 0x10,
        0x00, 0x00, 0x00, 0xFF, //replace last with length
        0x01, 0x00, 0x00, 0x00
              //Add data + EOF here
          };

        //public PacketService(string command)
        //{
        //    Command = command;
        //}

        internal string CustomData { get; set; }

        public string Command { get; set; }

        public int Length
        {
            get { return cmdBytes.Length; }
        }

        public void SetCommand(string command)
        {
            Command = command;
        }

        private byte[] cmdBytes
        {
            get { return Encoding.ASCII.GetBytes(Command); }
        }

        public byte[] GetBytes()
        {
            List<byte> ret = PacketTemplate.ToList();
            ret[11] = byte.Parse(string.Format("{0:X2}", (Length + 1).ToString()), NumberStyles.HexNumber);
            ret.AddRange(cmdBytes);
            ret.Add(EOF);

            return ret.ToArray();
        }

        public ICommand ParsePacket(string packetstring)
        {
            //string direction = packetstring.Substring(1, 1);
            string group = packetstring.Substring(2, 3);
            string command = packetstring.Substring(5);
            switch (group)
            {
                case "MVL":
                    //return MasterVolume.ParsePacket(command);
                    return new MasterVolumeCommand().ParsePacket(command);
                case "PWR":
                    //return Power.ParsePacket(command);
                    return new PowerCommand().ParsePacket(command);
                case "AMT":
                    //return Muting.ParsePacket(command);
                    return new MutingCommand().ParsePacket(command);
                case "IFA":
                    //return Audio.ParsePacket(command);
                    return new AudioCommand().ParsePacket(command);
                case "SLI":
                    //return Input.ParsePacket(packetstring);
                    return new InputCommand().ParsePacket(command);
            }
            throw new ArgumentException("Cannot find command-group " + group, "packetstring");
        }
    }
}
