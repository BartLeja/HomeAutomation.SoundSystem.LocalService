﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    public class DeviceDiscoveryService : IDeviceDiscoveryService
    {
        /* "!xECNQSTN" */
        private static UdpClient _client;
        private Socket _sock;
        private static IPEndPoint _localEndPoint;
        private IAddressResolutionProtocolService _addressResolutionProtocolService;
        private IPacketService _packetService;

        public DeviceDiscoveryService(
            IAddressResolutionProtocolService addressResolutionProtocolService,
            IPacketService packetService
            )
        {
            _addressResolutionProtocolService = addressResolutionProtocolService;
            _packetService = packetService;
        }

        public async Task<DiscoveryResult> DiscoverDevice(int port)
        {
            IEnumerable<string> ips = getInterfaceAddresses();
            var ret = new DiscoveryResult();
            _localEndPoint = new IPEndPoint(IPAddress.Any, 22222);
          
            _client = new UdpClient(port);
            _client.EnableBroadcast = true;
            _packetService.SetCommand("!xECNQSTN");
            byte[] sendbuf = _packetService.GetBytes();
            foreach (string networkaddress in ips)
            {
                try
                {
                    _client.Send(sendbuf, sendbuf.Length, IPAddress.Parse(networkaddress).ToString(), port);
                    Console.WriteLine("Dobry socket");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Zly socket");
                    Console.WriteLine(e);
                }
            }
            //while (_client.Available > 0)
            while (_client.Available > 0)
            {
              //  var recv = new byte[256];
               // _sock.Receive(recv, recv.Length, SocketFlags.None);
                 byte[] recv = _client.Receive(ref _localEndPoint);
                Thread.Sleep(300);
                var sb = new StringBuilder();
                foreach (byte t in recv)
                    sb.Append(char.ConvertFromUtf32(Convert.ToInt32(string.Format("{0:x2}", t), 16)));
                string stringData = sb.ToString();
               
                
                if (stringData.Contains("!1ECN"))
                {
                    int idx = stringData.IndexOf("!1ECN") + 5;
                    string[] parts = stringData.Substring(idx).Split('/');
                    string mac = parts[3].Substring(0, 12);
                    var ip = _addressResolutionProtocolService.GetIPInfo(mac);
                    ret.IP = ip.IpAddress;
                    ret.Port = Convert.ToInt32(parts[1]);
                    ret.Region = stringData.Substring(idx + 14, 2);
                    ret.MAC = mac;
                    ret.Model = stringData.Substring(idx, 7);
                }
            }
          //  _sock.Close();
            _client.Close();
            return ret;
        }

        //"!cECNnnnnnn/ppppp/dd/iiiiiiiiiiii"
        //dd:
        // DX: North American model
        // XX: European or Asian model
        // JJ: Japanese model

        private static IEnumerable<string> getInterfaceAddresses()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            var testtest = nics[0].GetIPProperties();

            var test = (from nic in nics
                    select nic.GetIPProperties()
                      into ipProps
                    from addr in ipProps.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    select GetBroadcastAddress(addr.Address, addr.IPv4Mask)
                        into network
                    where network != null
                    select network.ToString()).ToList();

            return test;
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            if (subnetMask == null) return null;
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public struct DiscoveryResult
        {
            public string MAC { get; set; }
            public string IP { get; set; }
            public int Port { get; set; }
            public string Model { get; set; }
            public string Region { get; set; }

            public override string ToString()
            {
                return string.Format("Model: {0}, Region: {1}, MAC: {2}\r\nIP: {3}, Port: {4}",
                                     Model,
                                     (Region == "DX")
                                       ? "North America"
                                       : (Region == "XX") ? "Europe/Asia" : (Region == "N/A") ? "N/A" : "Japan",
                                     MAC,
                                     IP,
                                     Port);
            }
        }
    }
}
