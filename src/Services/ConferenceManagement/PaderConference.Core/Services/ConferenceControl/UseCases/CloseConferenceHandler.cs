using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.ConferenceControl.Requests;

namespace PaderConference.Core.Services.ConferenceControl.UseCases
{
    public class CloseConferenceHandler : IRequestHandler<CloseConferenceRequest, SuccessOrError>
    {
        public CloseConferenceHandler(IPermissionsService permissionsService)
        {
        }

        public Task<SuccessOrError> Handle(CloseConferenceRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<SuccessOrError> VerifyPermissions()
        {
            
        }
    }
}
