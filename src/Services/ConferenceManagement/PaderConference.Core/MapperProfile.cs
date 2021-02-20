using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.Services;

namespace PaderConference.Core
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ParticipantData, ParticipantDto>();
        }
    }
}
