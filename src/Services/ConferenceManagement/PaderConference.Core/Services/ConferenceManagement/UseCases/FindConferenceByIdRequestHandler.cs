using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Services.ConferenceManagement.Gateways;
using PaderConference.Core.Services.ConferenceManagement.Requests;

namespace PaderConference.Core.Services.ConferenceManagement.UseCases
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
