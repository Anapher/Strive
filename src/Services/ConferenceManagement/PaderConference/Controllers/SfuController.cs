using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaderConference.Config;
using PaderConference.Messaging.SFU;
using PaderConference.Messaging.SFU.Dto;

namespace PaderConference.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class SfuController : Controller
    {
        private readonly ISfuConferenceInfoProvider _provider;
        private readonly SfuOptions _options;

        public SfuController(ISfuConferenceInfoProvider provider, IOptions<SfuOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        [HttpGet("{conferenceId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SfuConferenceInfo>> Fetch(string conferenceId, [FromQuery] string? apiKey)
        {
            if (apiKey != _options.ApiKey)
                return Forbid();

            return await _provider.Get(conferenceId);
        }
    }
}
