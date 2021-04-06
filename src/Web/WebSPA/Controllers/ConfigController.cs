using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebSPA.Utils;

namespace WebSPA.Controllers
{
    public class ConfigController : Controller
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly AppSettings _settings;

        public ConfigController(IOptionsSnapshot<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        [Route("config.js")]
        public IActionResult Configuration()
        {
            return new JavaScriptResult($"window.ENV = {JsonSerializer.Serialize(_settings, Options)};");
        }
    }
}
