using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Infrastructure.Services
{
    public interface IServiceInvokerFactory
    {
        IServiceInvoker CreateForHub(Hub hub);
    }
}
