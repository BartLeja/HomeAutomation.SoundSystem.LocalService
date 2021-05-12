using ArpLookup;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services
{
    /// <summary>
    /// This service allows you to retrieve the IP Address and Host Name for a specific machine on the local network when you only know it's MAC Address.
    /// </summary>
    public class AddressResolutionProtocolService : IAddressResolutionProtocolService
    {
        public string MacAddress { get; private set; }
        public string IPAddress { get; private set; }

        public async Task<DeviceAddressInfo> GetIPInfo(string macAddress, IEnumerable<string> ips)
        {
            foreach (var ip in ips)
            {
                try
                {
                    PhysicalAddress mac = await Arp.LookupAsync(System.Net.IPAddress.Parse(ip));
                    if (mac.ToString() == macAddress)
                       return new DeviceAddressInfo(macAddress, ip);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            return null;
        }


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
                //  var tewst = TryGetMacAddressOnLinux("");
                // Return list of IPInfo objects containing MAC / IP Address combinations
                return GetARPResult()
                    .Split(new[] { '\n', '\r' })
                    .Where(arp => 
                        !string.IsNullOrEmpty(arp) && 
                        GetIPRegex(arp) !=null && 
                        GetMacAddressRegex(arp) !=null)
                    .Select(dai => new DeviceAddressInfo(
                        GetMacAddressRegex(dai).Replace("-", string.Empty).Replace(":", string.Empty), 
                        GetIPRegex(dai)))
                    .ToList();
                
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
               if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
               {
                    p = Process.Start(new ProcessStartInfo("arp", "-a")
                    {
                         CreateNoWindow = true,
                         UseShellExecute = false,
                         RedirectStandardOutput = true
                    });
               }
               else
               {
                  
                    Console.WriteLine("linux arp");

                    try
                    {
                        p = Process.Start(new ProcessStartInfo("/bin/bash", "-c /usr/sbin/arp -?")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        });
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e);
                    }
                   
                }
                   
               output = p.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                p.Close();
           }
           catch (Exception ex)
           {
              throw new Exception("IPInfo: Error Retrieving 'arp -a' Results", ex);
           }
           finally
           {
               if (p != null)
                 p.Close();
           }

           return output;
        }

        private string GetIPRegex(string line)
        {
            string pattern = @"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b";
            return GetValueByRegex(line, pattern);
        }

        private string GetMacAddressRegex(string line)
        {
            var pattern = @"[0-9a-f]{2}[:-][0-9a-f]{2}[:-][0-9a-f]{2}[:-][0-9a-f]{2}[:-][0-9a-f]{2}[:-][0-9a-f]{2}";
            return GetValueByRegex(line,pattern);
        }

        private string GetValueByRegex(string line, string pattern)
        {
            var result = Regex.Matches(line, pattern, RegexOptions.IgnoreCase);
            return result.Count > 0 ? result.FirstOrDefault().Value : null;
        }
    }
}
