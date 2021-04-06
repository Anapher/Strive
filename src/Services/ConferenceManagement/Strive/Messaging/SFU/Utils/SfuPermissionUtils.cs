using System.Linq;
using System.Threading.Tasks;
using Strive.Core.Extensions;
using Strive.Core.Services.Permissions;
using Strive.Messaging.SFU.Dto;
using PermissionsDict = System.Collections.Generic.Dictionary<string, Newtonsoft.Json.Linq.JValue>;

namespace Strive.Messaging.SFU.Utils
{
    public static class SfuPermissionUtils
    {
        public static async ValueTask<SfuParticipantPermissions> MapToSfuPermissions(PermissionsDict permissionsDict)
        {
            var stack = new CachedPermissionStack(permissionsDict.Yield().ToList());
            var audio = await stack.GetPermissionValue(DefinedPermissions.Media.CanShareAudio);
            var webcam = await stack.GetPermissionValue(DefinedPermissions.Media.CanShareWebcam);
            var screen = await stack.GetPermissionValue(DefinedPermissions.Media.CanShareScreen);

            return new SfuParticipantPermissions(audio, webcam, screen);
        }
    }
}
