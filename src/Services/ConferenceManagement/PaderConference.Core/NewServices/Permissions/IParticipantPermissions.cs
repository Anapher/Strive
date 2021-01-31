using System.Threading.Tasks;

namespace PaderConference.Core.NewServices.Permissions
{
    public interface IParticipantPermissions
    {
        ValueTask<IPermissionStack> FetchForParticipant(string conferenceId, string participantId);
    }
}
