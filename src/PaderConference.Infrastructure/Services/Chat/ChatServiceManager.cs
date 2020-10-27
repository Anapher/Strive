using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderConference.Infrastructure.Services.Permissions;
using PaderConference.Infrastructure.Services.Synchronization;

namespace PaderConference.Infrastructure.Services.Chat
{
    public class ChatServiceManager : ConferenceServiceManager<ChatService>
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IMapper _mapper;
        private readonly IConferenceServiceManager<SynchronizationService> _synchronizationServiceManager;
        private readonly IConferenceServiceManager<PermissionsService> _permissionsServiceManager;
        private readonly IOptions<ChatOptions> _options;

        public ChatServiceManager(IMapper mapper,
            IConferenceServiceManager<SynchronizationService> synchronizationServiceManager,
            IConferenceServiceManager<PermissionsService> permissionsServiceManager, IOptions<ChatOptions> options,
            ILogger<ChatService> logger)
        {
            _mapper = mapper;
            _synchronizationServiceManager = synchronizationServiceManager;
            _permissionsServiceManager = permissionsServiceManager;
            _options = options;
            _logger = logger;
        }

        protected override async ValueTask<ChatService> ServiceFactory(string conferenceId)
        {
            var permissionsService = await _permissionsServiceManager.GetService(conferenceId);
            var synchronizeService = await _synchronizationServiceManager.GetService(conferenceId);

            return new ChatService(conferenceId, _mapper, permissionsService, synchronizeService, _options, _logger);
        }
    }
}