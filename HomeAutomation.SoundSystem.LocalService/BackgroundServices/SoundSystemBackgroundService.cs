using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HomeAutomation.SoundSystem.LocalService.Clients;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HomeAutomation.SoundSystem.LocalService.BackgroundServices
{
    public class SoundSystemBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ISignalRClient _signalRClient;
        private readonly ISoundControllerApi _soundControllerApi;

        public SoundSystemBackgroundService(
            IConfiguration configuration,
            ISignalRClient signalRClient,
            ISoundControllerApi soundControllerApi
        )
        {
            _configuration = configuration;
            _signalRClient = signalRClient;
            _soundControllerApi = soundControllerApi;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // var auth = configuration.Value.AuthorizationCredentials;
            var signalRHubUrl = _configuration.GetSection("SignalRHubUrl").Value;
            //var token = await restClient.GetToken();
            await _signalRClient.ConnectToSignalR("test", signalRHubUrl);
            var onkyoUrl = _configuration.GetSection("OnkyoUrl").Value;
            await _soundControllerApi.StartSoundApi(onkyoUrl);
        }
    }
}
