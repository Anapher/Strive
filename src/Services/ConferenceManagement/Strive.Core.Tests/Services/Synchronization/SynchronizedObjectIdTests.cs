using System.Collections.Generic;
using Strive.Core.Services.Synchronization;
using Xunit;

namespace Strive.Core.Tests.Services.Synchronization
{
    public class SynchronizedObjectIdTests
    {
        public static TheoryData<string, SynchronizedObjectId> SyncObjSerializationTestData = new()
        {
            {"test", new SynchronizedObjectId("test")},
            {"hello_world", new SynchronizedObjectId("hello_world")},
            {
                "hello_world?participantId=123",
                new SynchronizedObjectId("hello_world", new Dictionary<string, string> {{"participantId", "123"}})
            },
            {
                "test?a=1&b=2",
                new SynchronizedObjectId("test", new Dictionary<string, string> {{"a", "1"}, {"b", "2"}})
            },
        };

        [Theory]
        [MemberData(nameof(SyncObjSerializationTestData))]
        public void Parse_TestValues_DataShouldBeParsedToObject(string s, SynchronizedObjectId expected)
        {
            var actual = SynchronizedObjectId.Parse(s);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Parameters, actual.Parameters);
        }

        [Theory]
        [MemberData(nameof(SyncObjSerializationTestData))]
        public void ToString_TestValues_ReturnsCorrectString(string expected, SynchronizedObjectId syncObj)
        {
            var actual = syncObj.ToString();
            Assert.Equal(expected, actual);
        }
    }
}
