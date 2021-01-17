using System;
using System.Threading.Tasks;
using HashidsNet;
using MediatR;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.UseCases;
using PaderConference.Core.Notifications;

namespace PaderConference.Core.UseCases
{
    public class CreateConferenceUseCase : ICreateConferenceUseCase
    {
        private readonly IConferenceRepo _repo;
        private readonly IMediator _mediator;

        public CreateConferenceUseCase(IConferenceRepo repo, IMediator mediator)
        {
            _repo = repo;
            _mediator = mediator;
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

            await _mediator.Publish(new ConferenceJoinedNotification(id, message.ParticipantId));
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