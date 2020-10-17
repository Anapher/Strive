using System;

namespace PaderConference.Infrastructure.Conferencing
{
    public class ConferenceNotFoundException : Exception
    {
        public ConferenceNotFoundException(string conferenceId) : base($"The conference {conferenceId} was not found.")
        {
            ConferenceId = conferenceId;
        }

        public string ConferenceId { get; }
    }
}