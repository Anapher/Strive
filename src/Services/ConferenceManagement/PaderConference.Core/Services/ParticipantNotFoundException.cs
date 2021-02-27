using System;

namespace PaderConference.Core.Services
{
    public class ParticipantNotFoundException : Exception
    {
        public ParticipantNotFoundException(Participant participant) : base(
            $"The participant {participant} was not found.")
        {
            Participant = participant;
        }

        public Participant Participant { get; }
    }
}
