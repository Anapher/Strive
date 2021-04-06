using MediatR;
using Strive.Core.Domain.Entities;

namespace Strive.Core.Notifications
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
