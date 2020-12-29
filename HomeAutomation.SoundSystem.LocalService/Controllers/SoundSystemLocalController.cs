using HomeAutomation.SoundSystem.LocalService.OnkyoApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeAutomation.SoundSystem.LocalService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SoundSystemLocalController : ControllerBase
    {
        private ISoundControllerApi _onkyoApi;

        private readonly ILogger<SoundSystemLocalController> _logger;

        public SoundSystemLocalController(ILogger<SoundSystemLocalController> logger, ISoundControllerApi onkyoApi)
        {
            _logger = logger;
            _onkyoApi = onkyoApi;
        }

        [HttpGet]
        public string Get()
        {
            return "Sound System Local => is working";
        }
    }
}
