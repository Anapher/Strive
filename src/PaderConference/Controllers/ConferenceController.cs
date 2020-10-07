using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Extensions;
using PaderConference.Infrastructure.Extensions;
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
        public async Task<ActionResult<StartConferenceResponseDto>> Create([FromBody] StartConferenceRequestDto request,
            [FromServices] IStartConferenceUseCase useCase)
        {
            var result =
                await useCase.Handle(new StartConferenceRequest(User.GetUserId(),
                    request.Settings ?? new ConferenceSettings()));

            if (useCase.HasError) return useCase.ToActionResult();

            return new StartConferenceResponseDto(result!.ConferenceId);
        }
    }
}