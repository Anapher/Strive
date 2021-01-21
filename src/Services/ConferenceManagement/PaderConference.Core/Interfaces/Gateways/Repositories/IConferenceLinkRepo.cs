using System.Collections.Generic;
using System.Threading.Tasks;
using PaderConference.Core.Domain.Entities;
using SpeciVacation;

namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public interface IConferenceLinkRepo
    {
        Task<IReadOnlyList<ConferenceLink>> FindAsync(ISpecification<ConferenceLink> specification);

        Task<OptimisticUpdateResult> CreateOrReplaceAsync(ConferenceLink conferenceLink);

        Task DeleteAsync(ConferenceLink conferenceLink);
    }
}
