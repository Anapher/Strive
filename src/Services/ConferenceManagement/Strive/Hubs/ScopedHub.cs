using Autofac;
using Microsoft.AspNetCore.SignalR;

namespace Strive.Hubs
{
    public abstract class ScopedHub : Hub
    {
        protected ILifetimeScope HubScope;

        protected ScopedHub(ILifetimeScope scope)
        {
            HubScope = scope.BeginLifetimeScope();
        }

        protected override void Dispose(bool disposing)
        {
            // Dispose the hub lifetime scope when the hub is disposed.
            if (disposing) HubScope.Dispose();

            base.Dispose(disposing);
        }
    }
}
