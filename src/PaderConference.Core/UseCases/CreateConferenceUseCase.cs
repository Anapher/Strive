using System;
using System.Linq;
using System.Threading.Tasks;
using HashidsNet;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.UseCaseRequests;
using PaderConference.Core.Dto.UseCaseResponses;
using PaderConference.Core.Errors;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Interfaces.Services;
using PaderConference.Core.Interfaces.UseCases;

namespace PaderConference.Core.UseCases
{
    public class CreateConferenceUseCase : UseCaseStatus<CreateConferenceResponse>, ICreateConferenceUseCase
    {
        private readonly IConferenceRepo _repo;
        private readonly IConferenceScheduler _scheduler;

        public CreateConferenceUseCase(IConferenceRepo repo, IConferenceScheduler scheduler)
        {
            _repo = repo;
            _scheduler = scheduler;
        }

        public async ValueTask<CreateConferenceResponse?> Handle(CreateConferenceRequest message)
        {
            if (!message.Organizers.Any())
                return ReturnError(
                    new FieldValidationError(nameof(message.Organizers), "Organizers must not be empty."));

            var id = GenerateId();
            var conference = new Conference(id, message.Organizers)
            {
                Name = message.Name,
                StartTime = message.StartTime,
                ScheduleCron = message.ScheduleCron,
                ConferenceType = message.ConferenceType,
                Permissions = message.Permissions
            };

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

            await _scheduler.ScheduleConference(conference, true);
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