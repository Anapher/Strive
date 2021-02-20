using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services;

namespace PaderConference.Core.Interfaces.Services
{
    /// <summary>
    ///     The basic management of the conferences: open, close, add/remove participants
    /// </summary>
    public interface IConferenceManager
    {
        /// <summary>
        ///     Triggered after the conference was opened
        /// </summary>
        event EventHandler<Conference>? ConferenceOpened;

        /// <summary>
        ///     Triggered after the conference was closed. The event parameter is the id of the closed conference.
        /// </summary>
        event EventHandler<string>? ConferenceClosed;

        /// <summary>
        ///     Tries to open the conference. If the conference is already open, it does not throw an exception
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Returns the opened conference</returns>
        /// <exception cref="ConferenceNotFoundException">Triggered if the conference was not found.</exception>
        ValueTask<Conference> OpenConference(string conferenceId);

        /// <summary>
        ///     Close the conference. If the conference is already closed, it does not throw an exception
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        ValueTask CloseConference(string conferenceId);

        /// <summary>
        ///     Add a participant to the conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="participantId">The participant id</param>
        /// <param name="role">The role of the participant</param>
        /// <param name="displayName">The display name of the participant</param>
        /// <returns>Return the created participant</returns>
        /// <exception cref="ConferenceNotFoundException">Thrown if the conference was not found.</exception>
        /// <exception cref="InvalidOperationException">If the participant already participates in the conference.</exception>
        ValueTask<ParticipantData> Participate(string conferenceId, string participantId, string role,
            string? displayName);

        /// <summary>
        ///     Check if the conference is open
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return true if the conference is already open.</returns>
        ValueTask<bool> GetIsConferenceOpen(string conferenceId);

        /// <summary>
        ///     Get the participants of a conference
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <returns>Return the participants of the conference.</returns>
        ICollection<ParticipantData> GetParticipants(string conferenceId);

        /// <summary>
        ///     Remove a participant from a conference. If the participant was not found, do not throw an exception
        /// </summary>
        /// <param name="participantData">The participant id</param>
        ValueTask RemoveParticipant(ParticipantData participantData);

        /// <summary>
        ///     Get the conference of a participant
        /// </summary>
        /// <param name="participantData">The participant id</param>
        /// <returns>Return the conference id as string of the participant</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the participant is not in a conference</exception>
        string GetConferenceOfParticipant(ParticipantData participantData);

        /// <summary>
        ///     Try to get a participant from a conference. If the participant does not exist or is in a different conference,
        ///     return false.
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="participantId">The participant id</param>
        /// <param name="participant">The participant object.</param>
        /// <returns>Return true if the participant could be found.</returns>
        bool TryGetParticipant(string conferenceId, string participantId,
            [NotNullWhen(true)] out ParticipantData? participant);
    }
}