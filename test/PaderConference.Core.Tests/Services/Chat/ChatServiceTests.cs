using AutoMapper;

namespace PaderConference.Core.Tests.Services.Chat
{
    public class ChatServiceTests
    {
        private const string ConferenceId = "C_ID";

        public ChatServiceTests()
        {
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile<MapperProfile>()));
        }
    }
}
