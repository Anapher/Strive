using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Strive.Core.IntegrationTests._TestHelpers;
using Strive.Core.IntegrationTests.Services.Base;
using Strive.Core.Services.Media;
using Strive.Core.Services.Media.Dtos;
using Strive.Core.Services.Media.Requests;
using Strive.Core.Services.Synchronization.Requests;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.IntegrationTests.Services
{
    public class MediaTests : ServiceIntegrationTest
    {
        private const string ConferenceId = "123";

        public MediaTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IEnumerable<Type> FetchServiceTypes()
        {
            return FetchTypesForSynchronizedObjects().Concat(FetchTypesOfNamespace(typeof(SynchronizedMediaState)));
        }

        [Fact]
        public async Task ApplyMediaState_NoStateSet_UpdateSyncObj()
        {
            // arrange
            var testData = new Dictionary<string, ParticipantStreams>
            {
                {
                    "1",
                    new ParticipantStreams(
                        new Dictionary<string, ConsumerInfo>
                        {
                            {"453", new ConsumerInfo(false, "1", ProducerSource.Mic)},
                        },
                        new Dictionary<ProducerSource, ProducerInfo> {{ProducerSource.Mic, new ProducerInfo(false)}})
                },
            };

            // act
            await Mediator.Send(new ApplyMediaStateRequest(ConferenceId, testData));

            // assert
            var syncObj = (SynchronizedMediaState) await Mediator.Send(
                new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedMediaState.SyncObjId));

            Assert.NotEmpty(syncObj.Streams);
            TestEqualityHelper.AssertJsonConvertedEqual(testData, syncObj.Streams);
        }
    }
}
