using System.Collections.Generic;
using System.Threading.Tasks;

namespace Strive.Core.Services.Poll.Gateways
{
    public interface IPollRepository
    {
        ValueTask CreatePoll(string conferenceId, Poll poll);

        ValueTask DeletePollAndState(string conferenceId, string pollId);

        ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> DeletePollAnswers(string conferenceId, string pollId);

        ValueTask<PollState?> SetPollState(string conferenceId, string pollId, PollState state);

        ValueTask<IReadOnlyList<Poll>> GetPollsOfConference(string conferenceId);

        ValueTask<Poll?> GetPoll(string conferenceId, string pollId);

        ValueTask<PollState?> GetPollState(string conferenceId, string pollId);

        ValueTask<IReadOnlyDictionary<string, PollState>> GetStateOfAllPolls(string conferenceId);

        ValueTask SetPollAnswer(Participant participant, string pollId, PollAnswerWithKey answer);

        ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> GetPollAnswersOfParticipant(Participant participant,
            IEnumerable<string> pollIds);

        ValueTask<IReadOnlyDictionary<string, PollAnswerWithKey>> GetPollAnswers(string conferenceId, string pollId);

        ValueTask<PollAnswerWithKey?> GetPollAnswer(Participant participant, string pollId);
    }
}
