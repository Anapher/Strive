using System;
using System.Collections.Generic;

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
    }
}
