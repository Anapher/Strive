using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.Services;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Services.Permissions;
using PaderConference.Extensions;
using PaderConference.Models.Response;

namespace PaderConference.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ConferenceController : Controller
    {
        // POST api/v1/conference
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public async Task<ActionResult<StartConferenceResponseDto>> Create([FromBody] ConferenceData request,
            [FromServices] ICreateConferenceUseCase useCase)
        {
            var result = await useCase.Handle(new CreateConferenceRequest(request));
            if (!result.Success) return result.ToActionResult();

            return new StartConferenceResponseDto(result.Response.ConferenceId);
        }

        // PATCH api/v1/conference/{id}
        [HttpPatch("{conferenceId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public async Task<ActionResult> Patch(string conferenceId, [FromBody] JsonPatchDocument<ConferenceData> request,
            [FromServices] IPatchConferenceUseCase useCase)
        {
            var result = await useCase.Handle(new PatchConferenceRequest(request, conferenceId));

            if (!result.Success) return result.ToActionResult();
            return Ok();
        }

        // GET api/v1/conference/default-permissions
        [HttpGet("default-permissions")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = AppRoles.Moderator)]
        public ActionResult<Dictionary<PermissionType, IReadOnlyDictionary<string, JValue>>> GetDefaultPermissions(
            [FromServices] IOptions<DefaultPermissionOptions> options)
        {
            return options.Value.Default;
        }
    }
}