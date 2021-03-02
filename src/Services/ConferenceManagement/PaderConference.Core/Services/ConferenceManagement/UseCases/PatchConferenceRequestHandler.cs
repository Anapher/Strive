using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Services.ConferenceManagement.Requests;

namespace PaderConference.Core.Services.ConferenceManagement.UseCases
{
    public class PatchConferenceRequestHandler : IRequestHandler<PatchConferenceRequest>
    {
        public Task<Unit> Handle(PatchConferenceRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
