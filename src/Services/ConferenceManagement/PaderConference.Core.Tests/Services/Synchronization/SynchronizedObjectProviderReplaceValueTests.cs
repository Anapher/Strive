using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization
{
    public class SynchronizedObjectProviderReplaceValueTests : SynchronizedObjectProviderTestsBase
    {
        protected override async Task<TestSyncObj> TriggerUpdate(ISynchronizedObjectProvider<TestSyncObj> provider,
            string conferenceId, TestSyncObj value)
        {
            return await provider.Update(conferenceId, value);
        }

        protected override void AssertUpdateStrategy(IValueUpdate<TestSyncObj> update)
        {
            var replaceValue = Assert.IsType<ReplaceValueUpdate<TestSyncObj>>(update);
            Assert.NotNull(replaceValue.Value);
        }
    }
}
