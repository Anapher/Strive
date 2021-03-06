using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Services;
using PaderConference.Core.Services.ConferenceManagement.Requests;
using PaderConference.Core.Specifications;
using SpeciVacation;

namespace PaderConference.Core.Notifications.Handlers
{
    public class ConferenceJoinedUpdateLinkHandler : INotificationHandler<ConferenceJoinedNotification>
    {
        private readonly IMediator _mediator;
        private readonly IConferenceLinkRepo _conferenceLinkRepo;

        public ConferenceJoinedUpdateLinkHandler(IMediator mediator, IConferenceLinkRepo conferenceLinkRepo)
        {
            _mediator = mediator;
            _conferenceLinkRepo = conferenceLinkRepo;
        }

        public async Task Handle(ConferenceJoinedNotification notification, CancellationToken cancellationToken)
        {
            var link = (await _conferenceLinkRepo.FindAsync(
                new ConferenceLinkByConference(notification.ConferenceId).And(
                    new ConferenceLinkByParticipant(notification.ParticipantId)))).FirstOrDefault();

            if (link == null)
            {
                Conference conference;
                try
                {
                    conference = await _mediator.Send(new FindConferenceByIdRequest(notification.ConferenceId),
                        cancellationToken);
                }
                catch (ConferenceNotFoundException)
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                link = new ConferenceLink(notification.ParticipantId, notification.ConferenceId);
                link.UpdateFromConference(conference);
            }

            link.OnJoined();
            await _conferenceLinkRepo.CreateOrReplaceAsync(link);
        }
    }
}
