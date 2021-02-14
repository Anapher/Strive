using MediatR;

namespace PaderConference.Core.Services.Synchronization.Requests
{
    public record UpdateSubscriptionsRequest(string ConferenceId, string ParticipantId) : IRequest;
}


//Chat-> is typing
//    Rooms
//Scenes

//    In Redis Sync object mapping zu participants
//alte sync objects


//    Konferenz, Moderatoren, Raum, Invididual
//eine oder mehrere Instanzen


//Subscribe SyncObjects von Client?
//    Jedes SyncObj einzigartige ID

//    SyncObj CanSubscribe(participant)
