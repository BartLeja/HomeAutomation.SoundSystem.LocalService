using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    /// <summary>
    /// This service allows you to retrieve the IP Address and Host Name for a specific machine on the local network when you only know it's MAC Address.
    /// </summary>
    public class AddressResolutionProtocolService : IAddressResolutionProtocolService
    {
        //private string _hostName = string.Empty;
        //private DeviceAddressInfo _deviceAddressInfo;

        public AddressResolutionProtocolService(
           // string macAddress, string ipAddress
           )
        {
            //MacAddress = macAddress;
            //IPAddress = ipAddress;
           // _deviceAddressInfo = deviceAddressInfo;
        }

        public string MacAddress { get; private set; }
        public string IPAddress { get; private set; }

        //public string HostName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_hostName))
        //        {
        //            try
        //            {
        //                // Retrieve the "Host Name" for this IP Address. This is the "Name" of the machine.
        //                _hostName = Dns.GetHostEntry(IPAddress).HostName;
        //            }
        //            catch
        //            {
        //                _hostName = string.Empty;
        //            }
        //        }
        //        return _hostName;
        //    }
        //}

        /// <summary>
        /// Retrieves the IPInfo for the machine on the local network with the specified MAC Address.
        /// </summary>
        /// <param name="macAddress">The MAC Address of the IPInfo to retrieve.</param>
        /// <returns></returns>
        public DeviceAddressInfo GetIPInfo(string macAddress)
        {
            return GetIPsInfo()
                .Where(dai => dai.MacAddress.ToLowerInvariant() == macAddress.ToLowerInvariant())
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the IPInfo for All machines on the local network.
        /// </summary>
        /// <returns></returns>
        private List<DeviceAddressInfo> GetIPsInfo()
        {
            try
            {
                // Return list of IPInfo objects containing MAC / IP Address combinations
                return (from arp in GetARPResult().Split(new[] { '\n', '\r' })
                        where !string.IsNullOrEmpty(arp)
                        select (from piece in arp.Split(new[] { ' ', '\t' })
                                where !string.IsNullOrEmpty(piece)
                                select piece).ToArray()
                          into pieces
                        where pieces.Length == 3
                        select new DeviceAddressInfo(pieces[1].Replace("-", string.Empty), pieces[0])).ToList();

                //return GetARPResult().Split(new[] { '\n', '\r' })
                //    .Where(arp=> !string.IsNullOrEmpty(arp))
                //    .Select(arp => arp.Split(new[] { ' ', '\t' })
                //            .Select(piece => piece.Where(p=> !string.IsNullOrEmpty(p)))
            }
            catch (Exception ex)
            {
                throw new Exception("IPInfo: Error Parsing 'arp -a' results", ex);
            }
        }

        /// <summary>
        /// This runs the "arp" utility in Windows to retrieve all the MAC / IP Address entries.
        /// </summary>
        /// <returns></returns>
        private string GetARPResult()
        {
            Process p = null;
            string output;

            try
            {
                p = Process.Start(new ProcessStartInfo("arp", "-a")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });

                output = p.StandardOutput.ReadToEnd();

                p.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("IPInfo: Error Retrieving 'arp -a' Results", ex);
            }
            finally
            {
                if (p != null)
                {
                    p.Close();
                }
            }

            return output;
        }
    }
}
