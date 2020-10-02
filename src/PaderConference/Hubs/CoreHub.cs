using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PaderConference.Hubs
{
    [Authorize]
    public class CoreHub : Hub
    {
    }
}
