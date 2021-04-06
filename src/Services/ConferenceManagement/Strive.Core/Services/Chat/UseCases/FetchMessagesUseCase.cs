using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.Chat.Channels;
using Strive.Core.Services.Chat.Gateways;
using Strive.Core.Services.Chat.Requests;

namespace Strive.Core.Services.Chat.UseCases
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
