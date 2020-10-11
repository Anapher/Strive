using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IParticipantPermissions> GetPermissions(Participant participant);
    }

    public class PermissionsService : ConferenceService, IPermissionsService
    {
        public ValueTask<IParticipantPermissions> GetPermissions(Participant participant)
        {
            return new ValueTask<IParticipantPermissions>(new AllPermissions());
        }
    }

    public class AllPermissions : IParticipantPermissions
    {
        public bool CanShareWebcam { get; } = true;
        public bool CanShareAudio { get; } = true;
        public bool CanShareScreen { get; } = true;
    }

    public interface IParticipantPermissions
    {
        bool CanShareWebcam { get; }

        bool CanShareAudio { get; }

        bool CanShareScreen { get; }
    }

    public static class ParticipantPermissionsExtensions
    {
        public static bool CanShareMedia(this IParticipantPermissions permissions)
        {
            return permissions.CanShareAudio || permissions.CanShareWebcam || permissions.CanShareScreen;
        }
    }
}