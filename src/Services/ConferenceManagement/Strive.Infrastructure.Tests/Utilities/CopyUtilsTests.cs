using System.Collections.Generic;
using Newtonsoft.Json;
using Strive.Core.Domain.Entities;
using Strive.Infrastructure.Utilities;
using Xunit;

namespace Strive.Infrastructure.Tests.Utilities
{
    public class CopyUtilsTests
    {
        [Fact]
        public void DeepClone_NullValue_ReturnNull()
        {
            string? value = null;
            var result = CopyUtils.DeepClone(value);

            Assert.Equal(value, result);
        }

        [Fact]
        public void DeepClone_Conference_ReturnEqualCopy()
        {
            var data = new Conference("123")
            {
                ConferenceId = "test",
                Version = 45,
                Configuration = {Moderators = new List<string> {"ghrd"}, ScheduleCron = "asd"},
            };

            var result = CopyUtils.DeepClone(data);

            Assert.Equal(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void DeepClone_Conference_ReturnDifferentObj()
        {
            var data = new Conference("123")
            {
                ConferenceId = "test",
                Version = 45,
                Configuration = {Moderators = new List<string> {"ghrd"}, ScheduleCron = "asd"},
            };

            var result = CopyUtils.DeepClone(data);
            data.Configuration.Moderators.Add("test");

            Assert.NotEqual(JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(result));
        }
    }
}
