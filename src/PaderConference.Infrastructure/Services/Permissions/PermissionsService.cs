using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IPermissionStack> GetPermissions(Participant participant);
    }

    public class PermissionsService : ConferenceService, IPermissionsService
    {
        public PermissionsService()
        {
            ConferencePermissions = ImmutableDictionary<string, JsonElement>.Empty;
        }

        public IImmutableDictionary<string, JsonElement> ConferencePermissions { get; }

        public ValueTask<IPermissionStack> GetPermissions(Participant participant)
        {
            return new ValueTask<IPermissionStack>(new PermissionStack());
        }

        public override ValueTask InitializeAsync()
        {
            return base.InitializeAsync();
        }
    }

    public static class ParticipantPermissionsExtensions
    {
        public static bool CanShareMedia(this IParticipantPermissions permissions)
        {
            return permissions.CanShareAudio || permissions.CanShareWebcam || permissions.CanShareScreen;
        }
    }
}