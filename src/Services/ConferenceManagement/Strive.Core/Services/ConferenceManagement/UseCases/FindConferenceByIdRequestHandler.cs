using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Services.ConferenceManagement.UseCases
{
    public class FindConferenceByIdRequestHandler : IRequestHandler<FindConferenceByIdRequest, Conference>
    {
        private readonly IConferenceRepo _conferenceRepo;

        public FindConferenceByIdRequestHandler(IConferenceRepo conferenceRepo)
        {
            _conferenceRepo = conferenceRepo;
        }

        public async Task<Conference> Handle(FindConferenceByIdRequest request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceRepo.FindById(request.ConferenceId);
            if (conference == null) throw new ConferenceNotFoundException(request.ConferenceId);

            return conference;
        }
    }
}
