using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaderConference.Core.IntegrationTests._TestHelpers;
using PaderConference.Core.IntegrationTests.Services.Base;
using PaderConference.Core.Services.Media;
using PaderConference.Core.Services.Media.Dtos;
using PaderConference.Core.Services.Media.Requests;
using PaderConference.Core.Services.Synchronization.Requests;
using Xunit;
using Xunit.Abstractions;

namespace PaderConference.Core.IntegrationTests.Services
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
                new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedMediaStateProvider.SyncObjId));

            Assert.NotEmpty(syncObj.Streams);
            TestEqualityHelper.AssertJsonConvertedEqual(testData, syncObj.Streams);
        }
    }
}
