using AutoMapper;
using PaderConference.Core.Domain.Entities;
using PaderConference.Core.Dto.Services;

//using PaderConference.Core.Services.Chat;
//using PaderConference.Core.Services.Chat.Dto;

namespace PaderConference
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //CreateMap<ChatMessage, ChatMessageDto>();
            CreateMap<ParticipantData, ParticipantDto>();
        }
    }
}