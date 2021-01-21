using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;

namespace PaderConference.Core.Interfaces.UseCases
{
    public interface IJoinConferenceUseCase : IUseCaseRequestHandler<JoinConferenceRequest, JoinConferenceResponse>
    {
    }
}
