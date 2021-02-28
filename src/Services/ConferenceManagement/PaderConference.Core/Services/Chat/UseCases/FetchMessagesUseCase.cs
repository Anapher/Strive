using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces.Gateways;
using PaderConference.Core.Services.Chat.Channels;
using PaderConference.Core.Services.Chat.Gateways;
using PaderConference.Core.Services.Chat.Requests;

namespace PaderConference.Core.Services.Chat.UseCases
{
    public class FetchMessagesUseCase : IRequestHandler<FetchMessagesRequest, EntityPage<ChatMessage>>
    {
        private readonly IChatRepository _chatRepository;

        public FetchMessagesUseCase(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<EntityPage<ChatMessage>> Handle(FetchMessagesRequest request,
            CancellationToken cancellationToken)
        {
            var (conferenceId, chatChannel, start, end) = request;
            var channelId = ChannelSerializer.Encode(chatChannel).ToString();

            return await _chatRepository.FetchMessages(conferenceId, channelId, start, end);
        }
    }
}
