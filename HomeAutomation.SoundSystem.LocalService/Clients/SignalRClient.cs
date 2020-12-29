using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.Clients
{
    public class SignalRClient : ISignalRClient
    {
        private HubConnection _connection;
        private ISoundControllerApi _onkyoApi;
        private int _currentVolume;

        public SignalRClient(ISoundControllerApi onkyoApi)
        {
            _onkyoApi = onkyoApi;
        }

        public async Task ConnectToSignalR(string token, string signalRHubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(signalRHubUrl)
                //,
                //options =>
                //{
                //    options.AccessTokenProvider = () => Task.FromResult(token);
                //})
                .WithAutomaticReconnect()
                .Build();

            _connection.Closed += async (error) =>
            {
                var connectionState = false;
                while (!connectionState)
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    try
                    {
                        await _connection.StartAsync();
                        connectionState = true;
                    }
                    catch (Exception ex)
                    {
                        connectionState = false;
                        Console.WriteLine(ex);
                    }
                }
            };

            _connection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                Debug.WriteLine($"Message from {user} revived. {message}");

                // _logger.Log($"Message from {user} recived. {message}", typeof(SignalRClient).Namespace, "");
            });

            _connection.On("MasterVolumeUp", () =>
            {
                _onkyoApi.MasterVolumeUp();
            });

            _connection.On("MasterVolumeDown", () =>
            {
                _onkyoApi.MasterVolumeDown();
            });

            _connection.On("PowerOff", () =>
            {
                _onkyoApi.PowerOff();
            });

            _connection.On("PowerOn", () =>
            {
                _onkyoApi.PowerOn();
            });

            try
            {
                await _connection.StartAsync();
                _onkyoApi.OnPacketRecieved += OnSoundSystemChanged;
                _onkyoApi.MasterVolumeStatus();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task OnSoundSystemChanged(ICommand command)
        {

           if(command.Name == "On")
           {
               await InvokeSendPowerOnMethod();
           }
           if (command.Name == "Standby")
           {
               await InvokeSendPowerOffMethod();
           }
           if (command.Name == "SetLevel")
           {
               await InvokeChangeVolumeMethod((command as MasterVolumeCommand).lvl);
           }
        }

        public async Task InvokeChangeVolumeMethod(int level)
        {
            try
            {
                await _connection.InvokeAsync("SendChangeVolume", level);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task InvokeSendPowerOffMethod()
        {
            try
            {
                await _connection.InvokeAsync("SendPowerOff");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task InvokeSendPowerOnMethod()
        {
            try
            {
                await _connection.InvokeAsync("SendPowerOn");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
