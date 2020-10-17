using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Extensions;
using PaderConference.Models.Request;
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
        public async Task<ActionResult<StartConferenceResponseDto>> Create(
            [FromBody] CreateConferenceRequestDto request, [FromServices] ICreateConferenceUseCase useCase)
        {
            var result =
                await useCase.Handle(new CreateConferenceRequest(request.Name, request.ConferenceType,
                    request.Moderators, request.StartTime, request.EndTime, request.ScheduleCron,
                    request.Permissions ?? ImmutableDictionary<string, string>.Empty,
                    request.DefaultRoomPermissions ?? ImmutableDictionary<string, string>.Empty,
                    request.ModeratorPermissions ?? ImmutableDictionary<string, string>.Empty));

            if (useCase.HasError) return useCase.ToActionResult();
            return new StartConferenceResponseDto(result!.ConferenceId);
        }
    }
}