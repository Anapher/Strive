using System;
using System.Threading.Tasks;
using MediatR;

namespace PaderConference.Core.Interfaces.Services
{
    public interface IScheduledMediator
    {
        ValueTask<string> Schedule<T>(T notification, DateTimeOffset scheduleDate) where T : class, INotification;

        ValueTask Remove<T>(string id) where T : class, INotification;
    }
}
