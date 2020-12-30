using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Rooms.Requests;

namespace PaderConference.Core.Services.Rooms
{
    public interface IRoomManagement
    {
        /// <summary>
        ///     The current state (all rooms and participants)
        /// </summary>
        ConferenceRooms State { get; }

        /// <summary>
        ///     Triggered when rooms are created
        /// </summary>
        event EventHandler<IReadOnlyList<Room>>? RoomsCreated;

        /// <summary>
        ///     Triggered when rooms are removed. The string refers to the room id
        /// </summary>
        event EventHandler<IReadOnlyList<string>>? RoomsRemoved;

        /// <summary>
        ///     Create new rooms
        /// </summary>
        /// <param name="rooms">The rooms to created</param>
        /// <returns>Return info about the created rooms</returns>
        Task<IReadOnlyList<Room>> CreateRooms(IReadOnlyList<CreateRoomMessage> rooms);

        /// <summary>
        ///     Remove the given rooms
        /// </summary>
        /// <param name="roomIds">The ids of the rooms that should be removed</param>
        Task RemoveRooms(IReadOnlyList<string> roomIds);

        /// <summary>
        ///     Set the room of a participant
        /// </summary>
        /// <param name="participantId">The participant id</param>
        /// <param name="roomId">The room id of the room the participant should be moved to</param>
        /// <exception cref="InvalidOperationException">Thrown if the room does not exist or is disabled</exception>
        Task<SuccessOrError> SetRoom(string participantId, string roomId);
    }
}
