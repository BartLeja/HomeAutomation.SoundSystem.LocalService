using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using HomeAutomation.SoundSystem.LocalService.OnkyoApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HomeAutomation.SoundSystem.LocalService.Clients;
using HomeAutomation.Core.Logger;
using HomeAutomation.SoundSystem.LocalService.BackgroundServices;

namespace HomeAutomation.SoundSystem.LocalService
{
    public class Startup
    {
        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<Configuration.Configuration>(_configuration.GetSection("Configuration"));
            services.Configure<Configuration.Configuration>(_configuration.GetSection("OnkyoUrl"));
            services.AddSingleton<IAddressResolutionProtocolService, AddressResolutionProtocolService>();
            services.AddSingleton<IDeviceDiscoveryService, DeviceDiscoveryService>();
            services.AddSingleton<IPacketService, PacketService>();
            services.AddSingleton<ISocketService, SocketService>();
            services.AddSingleton<ISoundControllerApi, OnkyoApi.OnkyoApi>();
            services.AddSingleton<ISignalRClient, SignalRClient>();
            services.AddHomeAutomationLoggers();
            services.AddHostedService<SoundSystemBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public  void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
