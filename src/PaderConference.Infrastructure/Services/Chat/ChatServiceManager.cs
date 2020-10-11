using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Extensions;

namespace PaderConference.Infrastructure.Services.Chat
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

        protected override ValueTask<ChatService> ServiceFactory(Conference conference,
            IEnumerable<IConferenceServiceManager> services)
        {
            return new ChatService(conference, _mapper, _logger).ToValueTask();
        }
    }
}