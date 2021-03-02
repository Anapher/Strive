using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Concurrency.Optimistic;
using PaderConference.Core;
using PaderConference.Core.Dto;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Specifications;
using PaderConference.Models.Request;
using PaderConference.Models.Response;
using PaderConference.Presenters;
using Polly;
using SpeciVacation;

namespace PaderConference.Controllers
{
    [Route("v1/conference-link")]
    [ApiController]
    public class ConferenceLinkController : Controller
    {
        // GET v1/conference-links
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<ConferenceLinkDto>>> GetConferenceLinks(
            [FromServices] IConferenceLinkPresenter presenter)
        {
            var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var result = await presenter.GetConferenceLinks(userId);

            return Ok(result);
        }

        // DELETE v1/conference-links/{conferenceId}
        [HttpDelete("{conferenceId}")]
        [Authorize]
        public async Task<ActionResult> DeleteConferenceLink(string conferenceId,
            [FromServices] IConferenceLinkRepo repo)
        {
            var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var conferenceLink =
                (await repo.FindAsync(
                    new ConferenceLinkByParticipant(userId).And(new ConferenceLinkByConference(conferenceId))))
                .FirstOrDefault();

            if (conferenceLink == null)
                return NotFound(new Error(ErrorType.NotFound.ToString(), "The conference link was not found",
                    "ConferenceLink_NotFound"));

            try
            {
                await repo.DeleteAsync(conferenceLink);
            }
            catch (MongoConcurrencyDeletedException)
            {
            }

            return Ok();
        }

        // PATCH v1/conference-links/{conferenceId}
        [HttpPatch("{conferenceId}")]
        [Authorize]
        public async Task<ActionResult> PatchConferenceLink(string conferenceId,
            JsonPatchDocument<ChangeConferenceLinkStarDto> patch, [FromServices] IConferenceLinkRepo repo,
            [FromServices] IOptions<ConcurrencyOptions> options)
        {
            var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            return await Policy<ActionResult>.Handle<MongoConcurrencyUpdatedException>()
                .RetryAsync(options.Value.RetryCount).ExecuteAsync(async () =>
                {
                    var conferenceLink =
                        (await repo.FindAsync(
                            new ConferenceLinkByParticipant(userId).And(new ConferenceLinkByConference(conferenceId))))
                        .FirstOrDefault();

                    if (conferenceLink == null)
                        return NotFound(new Error(ErrorType.NotFound.ToString(), "The conference link was not found",
                            "ConferenceLink_NotFound"));

                    var dto = new ChangeConferenceLinkStarDto {Starred = conferenceLink.Starred};
                    patch.ApplyTo(dto);

                    conferenceLink.Starred = dto.Starred;

                    try
                    {
                        await repo.CreateOrReplaceAsync(conferenceLink);
                    }
                    catch (MongoConcurrencyDeletedException)
                    {
                        return NotFound(new Error(ErrorType.NotFound.ToString(), "The conference link was not found",
                            "ConferenceLink_NotFound"));
                    }

                    return Ok();
                });
        }
    }
}
