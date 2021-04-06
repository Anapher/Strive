using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Strive.Core.Services.ConferenceControl;
using Strive.Core.Services.ParticipantsList.Gateways;
using Strive.Core.Services.ParticipantsList.Requests;

namespace Strive.Core.Services.ParticipantsList.UseCases
{
    public class
        FetchParticipantMetadataUseCase : IRequestHandler<FetchParticipantsMetadataRequest, ParticipantMetadata>
    {
        private readonly IParticipantMetadataRepository _participantMetadataRepository;

        public FetchParticipantMetadataUseCase(IParticipantMetadataRepository participantMetadataRepository)
        {
            _participantMetadataRepository = participantMetadataRepository;
        }

        public async Task<ParticipantMetadata> Handle(FetchParticipantsMetadataRequest request,
            CancellationToken cancellationToken)
        {
            var data = await _participantMetadataRepository.GetParticipantMetadata(request.Participant);
            if (data == null)
                throw new ParticipantNotFoundException(request.Participant);

            return data;
        }
    }
}
