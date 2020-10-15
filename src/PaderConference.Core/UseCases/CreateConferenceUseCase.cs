using System;
using System.Threading.Tasks;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class CreateConferenceUseCase : UseCaseStatus<CreateConferenceResponse>, ICreateConferenceUseCase
    {
        public ValueTask<CreateConferenceResponse?> Handle(CreateConferenceRequest message)
        {
            throw new NotImplementedException();
        }
    }
}