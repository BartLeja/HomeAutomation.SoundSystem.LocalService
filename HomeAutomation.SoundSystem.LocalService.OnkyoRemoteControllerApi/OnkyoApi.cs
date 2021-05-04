using HomeAutomation.Core.Logger;
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
            IPacketService packetService
           )
        {
            _socketService = socketService;
            _deviceDiscoveryService = deviceDiscoveryService;
            _packetService = packetService;
        }

        public async Task StartSoundApi(string onkyoUrl)
        {
           await Connect(onkyoUrl);
        }

        public async Task MasterVolumeUp() => _socketService.SendPacket(CommandGenerator.MasterVolumeCommandGenerator(MasterVolumeCommandEnum.Up));
      
        public void MasterVolumeDown()
            => _socketService.SendPacket(CommandGenerator.MasterVolumeCommandGenerator(MasterVolumeCommandEnum.Down));

        public void MasterVolumeStatus()
           => _socketService.SendPacket(CommandGenerator.MasterVolumeCommandGenerator(MasterVolumeCommandEnum.Status));

        public void PowerOn() 
            => _socketService.SendPacket(CommandGenerator.PowerCommandGenerator(PowerCommandEnum.On));
       
        public void PowerOff()
            => _socketService.SendPacket(CommandGenerator.PowerCommandGenerator(PowerCommandEnum.Standby));

        private async Task Connect(string onkyoUrl)
        {
            //Auto-discovery, or get IP by input
            Console.WriteLine("Finding reciever...");
            //var discovery = ISCPDeviceDiscovery.DiscoverDevice("172.16.40.255", 60128);
            var discovery = await _deviceDiscoveryService.DiscoverDevice(60128);
            string deviceip = discovery.IP;
            if (string.IsNullOrEmpty(deviceip))
            {
                Console.WriteLine("Finding reciever... failed.");
                Console.WriteLine("Please input IP of reciever: ");
                deviceip = onkyoUrl;
                discovery.IP = onkyoUrl;
                discovery.MAC = "00:09:B0:91:41:3D";
                discovery.Model = "N/A";
                discovery.Port = 60128;
                discovery.Region = "N/A";
                
               // deviceip = discovery.IP;
            }
            else
            {
                Console.WriteLine("Finding reciever... Success.");
                Console.WriteLine(discovery.ToString());
            }

            //Check if host is alive
            var p = new Ping();
            PingReply rep = p.Send(deviceip, 36000);
            while (rep != null && rep.Status != IPStatus.Success)
            {
                Console.WriteLine(string.Format(
                  "Cannot connect to Onkyo reciever ({0}). Sleeping 30sec", rep.Status));
                Thread.Sleep(30000);
                p.Send(deviceip, 36000);
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
                //return false;
            }

          //  return true;
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
