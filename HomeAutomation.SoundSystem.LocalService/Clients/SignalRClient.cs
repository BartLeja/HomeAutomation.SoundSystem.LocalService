using HomeAutomation.Core.Logger;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Commands;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.Clients
{
    public class SignalRClient : ISignalRClient
    {
        private HubConnection _connection;
        private ISoundControllerApi _onkyoApi;
        private ITelegramLogger _logger;
        private ILokiLogger _lokiLogger;
        private int _currentVolume;

        public SignalRClient(ISoundControllerApi onkyoApi, 
            ITelegramLogger logger,
            ILokiLogger lokiLogger)
        {
            _onkyoApi = onkyoApi;
            _logger = logger;
            _lokiLogger = lokiLogger;
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
                await _logger.SendMessage("Sound System SignalR Disconnected");
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
                        await _lokiLogger.SendMessage($"Sound System {ex}",LogLevel.Error);
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

            _connection.On("MasterVolumeUp", async () =>
            {
                await _lokiLogger.SendMessage($"Sound System MasterVolumeUp");
                await _onkyoApi.MasterVolumeUp();
            });

            _connection.On("MasterVolumeDown", async () =>
            {
                await _lokiLogger.SendMessage($"Sound System MasterVolumeDown");
                _onkyoApi.MasterVolumeDown();
            });

            _connection.On("PowerOff", async () =>
            {
                await _lokiLogger.SendMessage($"Sound System PowerOff");
                _onkyoApi.PowerOff();
            });

            _connection.On("PowerOn", async () =>
            {
                await _lokiLogger.SendMessage($"Sound System PowerOn");
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
                await _lokiLogger.SendMessage($"Sound System {ex}", LogLevel.Error);
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
                await _lokiLogger.SendMessage($"Sound System {ex}", LogLevel.Error);
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
                await _lokiLogger.SendMessage($"Sound System {ex}", LogLevel.Error);
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
                await _lokiLogger.SendMessage($"Sound System {ex}", LogLevel.Error);
                Console.WriteLine(ex);
            }
        }
    }
}
