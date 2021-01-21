using MediatR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Core.Notifications
{
    public class ConferenceUpdatedNotification : INotification
    {
        public ConferenceUpdatedNotification(Conference conference)
        {
            Conference = conference;
        }

        public Conference Conference { get; }
    }
}
