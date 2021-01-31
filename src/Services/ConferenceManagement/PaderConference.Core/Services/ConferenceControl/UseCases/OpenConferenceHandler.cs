using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.NewServices.Permissions;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class OpenConferenceHandler : IRequestHandler<OpenConferenceRequest, SuccessOrError>
    {
        private readonly IPermissionsService _permissionsService;
        private readonly IConferenceManager _conferenceManager;

        public OpenConferenceHandler(IPermissionsService permissionsService, IConferenceManager conferenceManager)
        {
            _permissionsService = permissionsService;
            _conferenceManager = conferenceManager;
        }

        public async Task<SuccessOrError> Handle(OpenConferenceRequest request, CancellationToken cancellationToken)
        {
            var permissionCheckResult = await VerifyPermissions(request.Meta.Sender);
            if (!permissionCheckResult.Success) return permissionCheckResult;

            await _conferenceManager.OpenConference(request.Meta.ConferenceId);
            return SuccessOrError.Succeeded;
        }

        private async Task<SuccessOrError> VerifyPermissions(Participant participant)
        {
            var permissions = await _permissionsService.GetPermissions(participant);
            if (!await permissions.GetPermissionValue(DefinedPermissions.Conference.CanOpenAndClose))
                return CommonError.PermissionDenied(DefinedPermissions.Conference.CanOpenAndClose);

            return SuccessOrError.Succeeded;
        }
    }
}
