using HomeAutomation.SoundSystem.LocalService.Clients;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeAutomation.SoundSystem.LocalService.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task RunSignalRClientAsync(
          this IApplicationBuilder applicationBuilder,
          IConfiguration configuration,
          ISignalRClient signalRClient
            //,
          //IRestClient restClient
            )
        {
            // var auth = configuration.Value.AuthorizationCredentials;
            var signalRHubUrl = configuration.GetSection("SignalRHubUrl").Value;
            //var token = await restClient.GetToken();
            await signalRClient.ConnectToSignalR("test", signalRHubUrl);
        }

        public static async Task RunSoundSystemAsync(
          this IApplicationBuilder applicationBuilder,
            ISoundControllerApi soundControllerApi,
             IConfiguration configuration
            )
        {
            var onkyoUrl = configuration.GetSection("OnkyoUrl").Value;
            soundControllerApi.StartSoundApi(onkyoUrl);
        }
    }
}
