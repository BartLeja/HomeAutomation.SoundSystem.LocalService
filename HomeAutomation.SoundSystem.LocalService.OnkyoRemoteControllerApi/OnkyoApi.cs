using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi
{

    public delegate Task PacketRecievedApi(ICommand command);

    public class OnkyoApi : ISoundControllerApi
    {
        public  event PacketRecievedApi OnPacketRecieved;
        private ISocketService _socketService;
        private IDeviceDiscoveryService _deviceDiscoveryService;
        private IPacketService _packetService;

        private static bool _powerStatus;
        private static string _inputStatus;

        public OnkyoApi(
            ISocketService socketService, 
            IDeviceDiscoveryService deviceDiscoveryService,
            IPacketService packetService)
        {
            _socketService = socketService;
            _deviceDiscoveryService = deviceDiscoveryService;
            _packetService = packetService;
        }

        public void StartSoundApi()
        {
            connect();
        }

        public void MasterVolumeUp()
        {
            _socketService.SendPacket(new MasterVolumeCommand().Up);
        }

        public void MasterVolumeDown()
        {
            _socketService.SendPacket(new MasterVolumeCommand().Down);
        }

        public void PowerOn()
        {
            _socketService.SendPacket(new PowerCommand().On);
        }

        public void PowerOff()
        {
            _socketService.SendPacket(new PowerCommand().Off);
        }

        public void MasterVolumeStatus()
        {
          _socketService.SendPacket(new MasterVolumeCommand().Status);
        }

        private bool connect()
        {
            //Auto-discovery, or get IP by input
          
            Console.WriteLine("Finding reciever...");
            //var discovery = ISCPDeviceDiscovery.DiscoverDevice("172.16.40.255", 60128);
            var discovery = _deviceDiscoveryService.DiscoverDevice(60128);
            string deviceip = discovery.IP;
            if (string.IsNullOrEmpty(deviceip))
            {
                Console.WriteLine("Finding reciever... failed.");
                Console.WriteLine("Please input IP of reciever: ");
        
                discovery.IP = "http://192.168.1.147";
                discovery.MAC = "N/A";
                discovery.Model = "N/A";
                discovery.Port = 60128;
                discovery.Region = "N/A";
            }
            else
            {
                Console.WriteLine("Finding reciever... Success.");
                Console.WriteLine(discovery.ToString());
            }

            //Check if host is alive
            var p = new Ping();
            PingReply rep = p.Send(deviceip, 3000);
            while (rep != null && rep.Status != IPStatus.Success)
            {
                Console.WriteLine(string.Format(
                  "Cannot connect to Onkyo reciever ({0}). Sleeping 30sec", rep.Status));
                Thread.Sleep(30000);
                p.Send(deviceip, 3000);
            }

            //Setup sockets to reciever
            Console.WriteLine("Connecting.");
            _socketService.DeviceIp = discovery.IP;
            _socketService.DevicePort = discovery.Port;


            _socketService.OnPacketRecieved += ISCPSocket_OnPacketRecieved;
            try
            {
                _socketService.StartListener();
                Console.WriteLine("Connected!");
            }
            catch (Exception x)
            {
                Console.WriteLine("Error connecting (" + x.Message + ")");
                return false;
            }

            return true;
        }

        private void ISCPSocket_OnPacketRecieved(string str)
        {
            Console.WriteLine("Recieved: " + str);

            var r = _packetService.ParsePacket(str);
            if (r is PowerCommand)
            {
                _powerStatus = (r.Command == "!1PWR01");
            }
            else if (r is InputCommand)
            {
                _inputStatus = r.ToString();
            }
            OnPacketRecieved(r);
            Console.WriteLine(r.ToString());
        }
    }
}
