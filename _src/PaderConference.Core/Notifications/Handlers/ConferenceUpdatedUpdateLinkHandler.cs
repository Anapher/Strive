using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Specifications;

namespace PaderConference.Core.Notifications.Handlers
{
    public class ConferenceUpdatedUpdateLinkHandler : INotificationHandler<ConferenceUpdatedNotification>
    {
        private readonly IConferenceLinkRepo _conferenceLinkRepo;

        public ConferenceUpdatedUpdateLinkHandler(IConferenceLinkRepo conferenceLinkRepo)
        {
            _conferenceLinkRepo = conferenceLinkRepo;
        }

        public async Task Handle(ConferenceUpdatedNotification notification, CancellationToken cancellationToken)
        {
            List<string>? retry = null;
            while (retry == null || !retry.Any())
            {
                var failedUpdates = new List<string>();

                var links = await _conferenceLinkRepo.FindAsync(
                    new ConferenceLinkByConference(notification.Conference.ConferenceId));

                foreach (var conferenceLink in links)
                {
                    if (retry == null || retry.Contains(conferenceLink.Id))
                    {
                        conferenceLink.UpdateFromConference(notification.Conference);
                        var result = await _conferenceLinkRepo.CreateOrReplaceAsync(conferenceLink);
                        if (result == OptimisticUpdateResult.ConcurrencyException)
                            failedUpdates.Add(conferenceLink.Id);
                    }
                }

                retry = failedUpdates;
            }
        }
    }
}
