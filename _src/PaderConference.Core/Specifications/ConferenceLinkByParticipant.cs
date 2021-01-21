using System;
using System.Linq.Expressions;
using PaderConference.Core.Domain.Entities;
using SpeciVacation;

namespace PaderConference.Core.Specifications
{
    public class ConferenceLinkByParticipant : Specification<ConferenceLink>
    {
        private readonly string _participantId;

        public ConferenceLinkByParticipant(string participantId)
        {
            _participantId = participantId;
        }

        public override Expression<Func<ConferenceLink, bool>> ToExpression()
        {
            return link => link.ParticipantId == _participantId;
        }
    }
}
