//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using SpeciVacation;
//using Strive.Core.Domain.Entities;
//using Strive.Core.Interfaces.Gateways.Repositories;
//using Strive.Core.Services.ConferenceControl.Notifications;
//using Strive.Core.Services.ConferenceManagement.Requests;
//using Strive.Core.Specifications;

//namespace Strive.Core.Services.ConferenceLink.NotificationHandlers
//{
//    public class ConferenceJoinedUpdateLinkHandler : INotificationHandler<ParticipantJoinedNotification>
//    {
//        private readonly IMediator _mediator;
//        private readonly IConferenceLinkRepo _conferenceLinkRepo;

//        public ConferenceJoinedUpdateLinkHandler(IMediator mediator, IConferenceLinkRepo conferenceLinkRepo)
//        {
//            _mediator = mediator;
//            _conferenceLinkRepo = conferenceLinkRepo;
//        }

//        public async Task Handle(ParticipantJoinedNotification notification, CancellationToken cancellationToken)
//        {
//            var link = (await _conferenceLinkRepo.FindAsync(
//                new ConferenceLinkByConference(notification.Participant.ConferenceId).And(
//                    new ConferenceLinkByParticipant(notification.Participant.Id)))).FirstOrDefault();

//            if (link == null)
//            {
//                Conference conference;
//                try
//                {
//                    conference =
//                        await _mediator.Send(new FindConferenceByIdRequest(notification.Participant.ConferenceId),
//                            cancellationToken);
//                }
//                catch (ConferenceNotFoundException)
//                {
//                    return;
//                }

//                cancellationToken.ThrowIfCancellationRequested();

//                link = new Domain.Entities.ConferenceLink(notification.Participant.Id,
//                    notification.Participant.ConferenceId);
//                link.UpdateFromConference(conference);
//            }

//            link.OnJoined();
//            await _conferenceLinkRepo.CreateOrReplaceAsync(link);
//        }
//    }
//}

