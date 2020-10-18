using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Infrastructure.Hubs.Dto;
using PaderConference.Infrastructure.Services.Chat;

namespace PaderConference.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ChatMessage, ChatMessageDto>();
            CreateMap<Participant, ParticipantDto>();
        }
    }
}
