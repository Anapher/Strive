using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PaderConference.Infrastructure.Services.Permissions;

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

        protected override async ValueTask<ChatService> ServiceFactory(string conferenceId,
            IEnumerable<IConferenceServiceManager> services)
        {
            var permissionsService = await services.OfType<IConferenceServiceManager<PermissionsService>>().First()
                .GetService(conferenceId, services);

            return new ChatService(conferenceId, _mapper, permissionsService, _logger);
        }
    }
}