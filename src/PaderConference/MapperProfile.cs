using AutoMapper;
using PaderConference.Hubs.Chat;
using PaderConference.Models.Signal;

namespace PaderConference
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ChatMessage, ChatMessageDto>();
        }
    }
}