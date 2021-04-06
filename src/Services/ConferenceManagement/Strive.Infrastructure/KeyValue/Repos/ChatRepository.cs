using System.Collections.Generic;
using System.Threading.Tasks;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services;
using Strive.Core.Services.Chat;
using Strive.Core.Services.Chat.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class ChatRepository : IChatRepository, IKeyValueRepo
    {
        private const string PROP_KEY = "messages";
        private const string PROP_CHANNELS_KEY = "channels";
        private const string PROP_TYPING = "chatTyping";

        private readonly IKeyValueDatabase _database;

        public ChatRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask<int> AddChatMessageAndGetMessageCount(string conferenceId, string channel,
            ChatMessage message)
        {
            var chatKey = GetKey(conferenceId, channel);
            var channelsKey = GetChannelSetKey(conferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                _ = transaction.ListRightPushAsync(chatKey, message);
                _ = transaction.SetAddAsync(channelsKey, channel);
                var messagesCountTask = transaction.ListLenAsync(chatKey);

                await transaction.ExecuteAsync();
                return await messagesCountTask;
            }
        }

        public async ValueTask<EntityPage<ChatMessage>> FetchMessages(string conferenceId, string channel, int start,
            int end)
        {
            var key = GetKey(conferenceId, channel);

            using (var transaction = _database.CreateTransaction())
            {
                var messagesTask = transaction.ListRangeAsync<ChatMessage>(key, start, end);
                var totalMessagesCountTask = transaction.ListLenAsync(key);

                await transaction.ExecuteAsync();

                var messages = await messagesTask;
                var totalMessagesCount = await totalMessagesCountTask;

                return new EntityPage<ChatMessage>(messages!, totalMessagesCount);
            }
        }

        public async ValueTask DeleteChannel(string conferenceId, string channel)
        {
            var channelsKey = GetChannelSetKey(conferenceId);

            using (var transaction = _database.CreateTransaction())
            {
                _ = transaction.SetRemoveAsync(channelsKey, channel);
                RemoveChannel(transaction, conferenceId, channel);

                await transaction.ExecuteAsync();
            }
        }

        public async ValueTask<IReadOnlyList<string>> FetchAllChannels(string conferenceId)
        {
            var channelsKey = GetChannelSetKey(conferenceId);
            return await _database.SetMembersAsync(channelsKey);
        }

        public ValueTask<bool> AddParticipantTyping(Participant participant, string channel)
        {
            var key = GetTypingSetKey(participant.ConferenceId, channel);
            return _database.SetAddAsync(key, participant.Id);
        }

        public ValueTask<bool> RemoveParticipantTyping(Participant participant, string channel)
        {
            var key = GetTypingSetKey(participant.ConferenceId, channel);
            return _database.SetRemoveAsync(key, participant.Id);
        }

        public ValueTask<IReadOnlyList<string>> GetAllParticipantsTyping(string conferenceId, string channel)
        {
            var key = GetTypingSetKey(conferenceId, channel);
            return _database.SetMembersAsync(key);
        }

        public async ValueTask RemoveAllDataOfConference(string conferenceId)
        {
            var channelsKey = GetChannelSetKey(conferenceId);

            IReadOnlyList<string> channels;
            using (var transaction = _database.CreateTransaction())
            {
                var channelsTask = transaction.SetMembersAsync(channelsKey);
                _ = transaction.KeyDeleteAsync(channelsKey);

                await transaction.ExecuteAsync();

                channels = await channelsTask;
            }

            using (var transaction = _database.CreateTransaction())
            {
                foreach (var channel in channels)
                {
                    RemoveChannel(transaction, conferenceId, channel);
                }

                await transaction.ExecuteAsync();
            }
        }

        private void RemoveChannel(IKeyValueDatabaseActions actions, string conferenceId, string channel)
        {
            var messagesKey = GetKey(conferenceId, channel);
            var typingKey = GetKey(conferenceId, channel);

            _ = actions.KeyDeleteAsync(messagesKey);
            _ = actions.KeyDeleteAsync(typingKey);
        }

        private static string GetKey(string conferenceId, string channel)
        {
            return DatabaseKeyBuilder.ForProperty(PROP_KEY).ForConference(conferenceId).ForSecondary(channel)
                .ToString();
        }

        private static string GetChannelSetKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(PROP_CHANNELS_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetTypingSetKey(string conferenceId, string channel)
        {
            return DatabaseKeyBuilder.ForProperty(PROP_TYPING).ForConference(conferenceId).ForSecondary(channel)
                .ToString();
        }
    }
}
