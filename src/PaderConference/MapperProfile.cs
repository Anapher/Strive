using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Hubs.Chat;
using PaderConference.Models.Signal;

namespace PaderConference
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