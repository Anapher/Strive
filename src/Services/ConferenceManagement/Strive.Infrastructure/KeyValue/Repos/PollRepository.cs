using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Services;
using Strive.Core.Services.Poll;
using Strive.Core.Services.Poll.Gateways;
using Strive.Infrastructure.KeyValue.Abstractions;
using Strive.Infrastructure.KeyValue.Extensions;

namespace Strive.Infrastructure.KeyValue.Repos
{
    public class PollRepository : IPollRepository, IKeyValueRepo
    {
        private const string POLLS_PROPERTY_KEY = "polls";
        private const string POLLSTATE_PROPERTY_KEY = "pollState";
        private const string POLLANSWERS_PROPERTY_KEY = "pollAnswers";

        private readonly IKeyValueDatabase _database;

        public PollRepository(IKeyValueDatabase database)
        {
            _database = database;
        }

        public async ValueTask CreatePoll(string conferenceId, Poll poll)
        {
            var pollsKey = GetPollsKey(conferenceId);
            await _database.HashSetAsync(pollsKey, poll.Id, poll);
        }

        public async ValueTask DeletePollAndState(string conferenceId, string pollId)
        {
            var pollsKey = GetPollsKey(conferenceId);
            var stateKey = GetPollStateKey(conferenceId);

            using var transaction = _database.CreateTransaction();
            _ = transaction.HashDeleteAsync(pollsKey, pollId);
            _ = transaction.HashDeleteAsync(stateKey, pollId);

            await transaction.ExecuteAsync();
        }

        public async ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> DeletePollAnswers(string conferenceId,
            string pollId)
        {
            var key = GetPollAnswersKey(conferenceId, pollId);

            using var transaction = _database.CreateTransaction();
            var answersTask = _database.HashGetAllAsync<PollAnswerWithKey>(key);
            _ = transaction.KeyDeleteAsync(key);

            await transaction.ExecuteAsync();

            return (await answersTask)!;
        }

        public async ValueTask<PollState?> SetPollState(string conferenceId, string pollId, PollState state)
        {
            var stateKey = GetPollStateKey(conferenceId);

            using var transaction = _database.CreateTransaction();
            var previousPollTask = transaction.HashGetAsync<PollState>(stateKey, pollId);
            _ = transaction.HashSetAsync(stateKey, pollId, state);

            await transaction.ExecuteAsync();

            return await previousPollTask;
        }

        public async ValueTask<IReadOnlyList<Poll>> GetPollsOfConference(string conferenceId)
        {
            var pollsKey = GetPollsKey(conferenceId);
            var result = await _database.HashGetAllAsync<Poll>(pollsKey);
            return result.Values.ToList()!;
        }

        public async ValueTask<Poll?> GetPoll(string conferenceId, string pollId)
        {
            var pollsKey = GetPollsKey(conferenceId);
            return await _database.HashGetAsync<Poll>(pollsKey, pollId);
        }

        public async ValueTask<PollState?> GetPollState(string conferenceId, string pollId)
        {
            var stateKey = GetPollStateKey(conferenceId);
            return await _database.HashGetAsync<PollState>(stateKey, pollId);
        }

        public async ValueTask<IReadOnlyDictionary<string, PollState>> GetStateOfAllPolls(string conferenceId)
        {
            var stateKey = GetPollStateKey(conferenceId);
            return (await _database.HashGetAllAsync<PollState>(stateKey))!;
        }

        public async ValueTask SetPollAnswer(Participant participant, string pollId, PollAnswerWithKey answer)
        {
            var key = GetPollAnswersKey(participant.ConferenceId, pollId);
            await _database.HashSetAsync(key, participant.Id, answer);
        }

        public async ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> GetPollAnswersOfParticipant(
            Participant participant, IEnumerable<string> pollIds)
        {
            using var transaction = _database.CreateTransaction();

            var values = pollIds.ToDictionary(pollId => pollId,
                pollId => transaction.HashGetAsync<PollAnswerWithKey>(
                    GetPollAnswersKey(participant.ConferenceId, pollId), participant.Id));

            await transaction.ExecuteAsync();

            var result = new Dictionary<string, PollAnswerWithKey>();
            foreach (var (pollId, valueTask) in values)
            {
                var answer = await valueTask;
                if (answer == null) continue;

                result.Add(pollId, answer);
            }

            return result;
        }

        public async ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> GetPollAnswers(string conferenceId,
            string pollId)
        {
            var key = GetPollAnswersKey(conferenceId, pollId);
            return (await _database.HashGetAllAsync<PollAnswerWithKey>(key))!;
        }

        public async ValueTask<PollAnswerWithKey?> GetPollAnswer(Participant participant, string pollId)
        {
            var key = GetPollAnswersKey(participant.ConferenceId, pollId);
            return await _database.HashGetAsync<PollAnswerWithKey>(key, participant.Id);
        }

        private static string GetPollsKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(POLLS_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetPollStateKey(string conferenceId)
        {
            return DatabaseKeyBuilder.ForProperty(POLLSTATE_PROPERTY_KEY).ForConference(conferenceId).ToString();
        }

        private static string GetPollAnswersKey(string conferenceId, string pollId)
        {
            return DatabaseKeyBuilder.ForProperty(POLLANSWERS_PROPERTY_KEY).ForConference(conferenceId)
                .ForSecondary(pollId).ToString();
        }
    }
}
