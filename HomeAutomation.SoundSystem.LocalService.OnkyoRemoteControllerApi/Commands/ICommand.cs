using System;
using System.Collections.Generic;
using System.Text;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands
{
    public interface ICommand
    {
        string Command { get; set; }
        string Name { get; set; }
        //T ParsePacket(string command);
        //  byte[] GetBytes(string command);
        byte[] GetBytes();
    }
}
