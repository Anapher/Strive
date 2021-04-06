using System;
using System.Threading;
using System.Threading.Tasks;
using HashidsNet;
using MediatR;
using Strive.Core.Domain.Entities;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;

namespace Strive.Core.Services.ConferenceManagement.UseCases
{
    public class CreateConferenceRequestHandler : IRequestHandler<CreateConferenceRequest, string>
    {
        private readonly IConferenceRepo _conferenceRepo;

        public CreateConferenceRequestHandler(IConferenceRepo conferenceRepo)
        {
            _conferenceRepo = conferenceRepo;
        }

        public async Task<string> Handle(CreateConferenceRequest request, CancellationToken cancellationToken)
        {
            var data = request.Data;

            var id = GenerateId();
            var conference = new Conference(id) {Permissions = data.Permissions, Configuration = data.Configuration};

            try
            {
                await _conferenceRepo.Create(conference);
            }
            // ReSharper disable once RedundantCatchClause
            catch (Exception e)
            {
                // todo: handle duplicate ids, 
                throw;
            }

            return id;
        }

        private static string GenerateId()
        {
            var guid = Guid.NewGuid();
            var id = Math.Abs(guid.GetHashCode());

            return new Hashids("Strive").Encode(id);
        }
    }
}
