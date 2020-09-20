using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public delegate void PacketRecieved(string str);

    public class SocketService : ISocketService
    {
        private  bool _shutdown;
        private  Socket _sock;
        private  Thread _listener;
        public string DeviceIp { get; set; }
        public  int DevicePort { get; set; }
        private  bool blocked { get; set; }
        public event PacketRecieved OnPacketRecieved;
       // private IPacketService _packetService;

        public SocketService(
           // IPacketService packetService
            )
        {
         //   _packetService = packetService;
        }

        public void SendPacket(ICommand packet, bool blocking = false)
        {
            if (blocking)
            {
                blocked = true;
                OnPacketRecieved -= blockingListen;
                OnPacketRecieved += blockingListen;
            }
            checkConnect();
            if (_sock != null && _sock.Connected)
            {
                _sock.Send(packet.GetBytes(), 0, packet.GetBytes().Length, SocketFlags.None);
            }
            while (blocked) Thread.Sleep(100);
            Thread.Sleep(100);
        }

        public void StartListener()
        {
            checkConnect();
            _shutdown = false;
            _listener = new Thread(socketListener);
            _listener.Start();
        }

        public void StopListener()
        {
            _shutdown = true;
        }

        public void Dispose()
        {
            _shutdown = true;
            _sock.Close();
            _sock.Dispose();
            try
            {
                _listener.Abort();
            }
            catch
            {
            }
        }

        private void socketListener()
        {
            try
            {
                while (!_shutdown)
                {
                    try
                    {
                        if (_sock.Available > 0)
                        {
                            var buffer = new byte[256];
                            _sock.Receive(buffer, buffer.Length, SocketFlags.None);
                            var builder = new StringBuilder();
                            for (int i = 16; buffer[i] != 26; i++)
                            {
                                if (buffer[i] != 26)
                                {
                                    int num = Convert.ToInt32(string.Format("{0:x2}", buffer[i]), 16);
                                    builder.Append(char.ConvertFromUtf32(num));
                                }
                            }
                            if (OnPacketRecieved != null)
                                OnPacketRecieved(builder.ToString());
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
            }
        }

        private void checkConnect()
        {
            try
            {
                if (_sock == null)
                    _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 1000 };
                if (!_sock.Connected)
                    _sock.Connect(DeviceIp, DevicePort);
            }
            catch (Exception x)
            {

            }
        }

        private  void blockingListen(string str)
        {
            blocked = false;
        }
    }
}
