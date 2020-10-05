using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs
{
    public interface IServiceMessage
    {
        Participant Participant { get; }

        HubCallerContext Context { get; }

        IHubCallerClients Clients { get; }
    }

    public interface IServiceMessage<out TPayload> : IServiceMessage
    {
        TPayload Payload { get; }
    }

    public class ServiceMessage : IServiceMessage
    {
        public ServiceMessage(Participant participant, HubCallerContext context, IHubCallerClients clients)
        {
            Participant = participant;
            Context = context;
            Clients = clients;
        }

        public Participant Participant { get; }
        public HubCallerContext Context { get; }
        public IHubCallerClients Clients { get; }
    }

    public class ServiceMessage<TPayload> : ServiceMessage, IServiceMessage<TPayload>
    {
        public ServiceMessage(TPayload payload, Participant participant, HubCallerContext context,
            IHubCallerClients clients) : base(participant, context, clients)
        {
            Payload = payload;
        }

        public TPayload Payload { get; }
    }

    public interface IConferenceService : IAsyncDisposable
    {
        ValueTask OnClientDisconnected(Participant participant);
    }

    public interface IConferenceServiceManager<out TService> where TService : IConferenceService
    {
        TService GetService(Conference conference);
        ValueTask Close(Conference conference);
    }

    public abstract class ConferenceServiceManager<TService> : IConferenceServiceManager<TService>
        where TService : IConferenceService
    {
        private readonly ConcurrentDictionary<Conference, TService> _services =
            new ConcurrentDictionary<Conference, TService>();

        public TService GetService(Conference conference)
        {
            if (!_services.TryGetValue(conference, out var service))
                return _services.GetOrAdd(conference, ServiceFactory);

            return service;
        }

        public ValueTask Close(Conference conference)
        {
            if (_services.TryRemove(conference, out var service))
                return service.DisposeAsync();

            return new ValueTask();
        }

        protected abstract TService ServiceFactory(Conference conference);
    }
}