using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Strive.Core.Dto.Services;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.Permissions.Options;
using Strive.Models.Response;

namespace Strive.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class ConferenceController : Controller
    {
        private readonly IMediator _mediator;

        public ConferenceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST v1/conference
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public async Task<ActionResult<ConferenceCreatedResponseDto>> Create([FromBody] ConferenceData data)
        {
            var conferenceId = await _mediator.Send(new CreateConferenceRequest(data), HttpContext.RequestAborted);
            return new ConferenceCreatedResponseDto(conferenceId);
        }

        // PATCH v1/conference/{id}
        [HttpPatch("{conferenceId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public async Task<ActionResult> Patch(string conferenceId, [FromBody] JsonPatchDocument<ConferenceData> patch)
        {
            await _mediator.Send(new PatchConferenceRequest(conferenceId, patch));
            return Ok();
        }

        // GET v1/conference/default-data
        [HttpGet("default-data")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public ActionResult<ConferenceData> GetDefault([FromServices] IOptions<DefaultPermissionOptions> options)
        {
            return new ConferenceData
            {
                Permissions = options.Value.Default.ToDictionary(x => x.Key,
                    x => x.Value.ToDictionary(y => y.Key, y => y.Value)),
            };
        }
    }
}