using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public abstract class Command<T> 
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

       // string ICommand.Command { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        //public string Command1 { get; set; }
        public abstract T ParsePacket(string command);

        private byte[] cmdBytes(string command)
        {
            return Encoding.ASCII.GetBytes(command); 
        }

        //public int Length
        //{
        //    get { return cmdBytes.Length; }
        //}

        public byte[] GetBytes(string command)
        {
            var cmdBytes = Encoding.ASCII.GetBytes(command);
            List<byte> ret = PacketTemplate.ToList();
            ret[11] = byte.Parse(string.Format("{0:X2}", (cmdBytes.Length + 1).ToString()), NumberStyles.HexNumber);
            ret.AddRange(cmdBytes);
            ret.Add(EOF);

            return ret.ToArray();
        }
    }
}
