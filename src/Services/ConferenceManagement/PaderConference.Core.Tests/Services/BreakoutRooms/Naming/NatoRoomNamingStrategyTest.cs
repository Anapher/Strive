using PaderConference.Core.Services.BreakoutRooms.Naming;
using Xunit;

namespace PaderConference.Core.Tests.Services.BreakoutRooms.Naming
{
    public class NatoRoomNamingStrategyTest
    {
        public static readonly TheoryData<int, string> TestData = new TheoryData<int, string>
        {
            {0, "Alpha"},
            {1, "Bravo"},
            {26, "Alpha #2"},
            {29, "Delta #2"},
            {52, "Alpha #3"},
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestGetName(int index, string expected)
        {
            var name = new NatoRoomNamingStrategy().GetName(index);
            Assert.Equal(expected, name);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestParseIndex(int expected, string name)
        {
            var index = new NatoRoomNamingStrategy().ParseIndex(name);
            Assert.Equal(expected, index);
        }
    }
}
