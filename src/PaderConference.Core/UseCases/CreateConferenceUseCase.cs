using System;
using System.Threading.Tasks;
using HashidsNet;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class CreateConferenceUseCase : ICreateConferenceUseCase
    {
        private readonly IConferenceRepo _repo;

        public CreateConferenceUseCase(IConferenceRepo repo)
        {
            _repo = repo;
        }

        public async ValueTask<SuccessOrError<CreateConferenceResponse>> Handle(CreateConferenceRequest message)
        {
            var data = message.Data;

            var id = GenerateId();
            var conference = new Conference(id) {Permissions = data.Permissions, Configuration = data.Configuration};

            try
            {
                await _repo.Create(conference);
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception e)
            {
                // todo: handle duplicate ids, 
                throw;
            }

            return new CreateConferenceResponse(id);
        }

        private static string GenerateId()
        {
            var guid = Guid.NewGuid();
            var id = Math.Abs(guid.GetHashCode());

            return new Hashids("PaderConference").Encode(id);
        }
    }
}