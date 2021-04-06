using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Services.ConferenceManagement.UseCases
{
    public class PatchConferenceRequestHandler : IRequestHandler<PatchConferenceRequest>
    {
        public Task<Unit> Handle(PatchConferenceRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
