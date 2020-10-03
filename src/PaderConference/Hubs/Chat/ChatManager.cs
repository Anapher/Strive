using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Sockets;

namespace PaderConference.Hubs.Chat
{
    public interface IChatManager
    {
        ConferenceChat GetChat(Conference conference);
    }

    public class ChatManager : IChatManager
    {
        private readonly ConcurrentDictionary<Conference, ConferenceChat> _chats =
            new ConcurrentDictionary<Conference, ConferenceChat>();

        private readonly IConnectionMapping _connectionMapping;
        private readonly ILogger<ConferenceChat> _logger;
        private readonly IMapper _mapper;


        public ChatManager(IConnectionMapping connectionMapping, IMapper mapper, ILogger<ConferenceChat> logger)
        {
            _connectionMapping = connectionMapping;
            _mapper = mapper;
            _logger = logger;
        }

        public ConferenceChat GetChat(Conference conference)
        {
            if (!_chats.TryGetValue(conference, out var conferenceChat))
                return _chats.GetOrAdd(conference, c => new ConferenceChat(c, _connectionMapping, _mapper, _logger));

            return conferenceChat;
        }
    }
}