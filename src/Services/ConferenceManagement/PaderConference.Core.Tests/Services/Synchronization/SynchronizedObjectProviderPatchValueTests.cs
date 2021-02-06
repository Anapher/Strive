using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization
{
    public class SynchronizedObjectProviderPatchValueTests : SynchronizedObjectProviderTestsBase
    {
        protected override async Task<TestSyncObj> TriggerUpdate(ISynchronizedObjectProvider<TestSyncObj> provider,
            string conferenceId, TestSyncObj value)
        {
            return await provider.Update(conferenceId, _ => value);
        }

        protected override void AssertUpdateStrategy(IValueUpdate<TestSyncObj> update)
        {
            var patchValue = Assert.IsType<PatchValueUpdate<TestSyncObj>>(update);
            Assert.NotNull(patchValue.UpdateStateFn);
        }
    }
}
