using System.Threading.Tasks;

namespace Strive.Core.Services.Permissions
{
    public interface IParticipantPermissions
    {
        ValueTask<IPermissionStack> FetchForParticipant(Participant participant);
    }
}
