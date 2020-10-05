using AutoMapper;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Hubs.Chat
{
    public class ChatServiceManager : ConferenceServiceManager<ChatService>
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IMapper _mapper;

        public ChatServiceManager(IMapper mapper, ILogger<ChatService> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        protected override ChatService ServiceFactory(Conference conference)
        {
            return new ChatService(conference, _mapper, _logger);
        }
    }
}