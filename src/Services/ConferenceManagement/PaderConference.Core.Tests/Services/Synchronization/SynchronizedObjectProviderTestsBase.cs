using System.Threading.Tasks;
using MediatR;
using Moq;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UpdateStrategy;
using PaderConference.Core.Tests._TestUtils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization
{
    public abstract class SynchronizedObjectProviderTestsBase
    {
        protected abstract Task<TestSyncObj> TriggerUpdate(ISynchronizedObjectProvider<TestSyncObj> provider,
            string conferenceId, TestSyncObj value);

        protected abstract void AssertUpdateStrategy(IValueUpdate<TestSyncObj> update);

        [Fact]
        public async Task Update_DefaultSyncObjValues_SendUpdateSynchronizedObjectRequest()
        {
            const string conferenceId = "test";
            const string syncObjName = nameof(TestSyncObj);

            // arrange
            var mediatorMock = new Mock<IMediator>();
            var capturedRequest =
                mediatorMock.CaptureRequest<UpdateSynchronizedObjectRequest<TestSyncObj>, TestSyncObj>();

            var initialValue = new TestSyncObj {TestVal = false};
            var syncObjProvider = new TestSyncObjProvider(mediatorMock.Object, initialValue);

            var newValue = new TestSyncObj {TestVal = true};

            // act
            await TriggerUpdate(syncObjProvider, conferenceId, newValue);

            // assert
            capturedRequest.AssertReceived();
            mediatorMock.VerifyNoOtherCalls();

            var request = capturedRequest.GetRequest();
            Assert.Equal(conferenceId, request.Metadata.ConferenceId);
            Assert.Equal(syncObjName, request.Metadata.Name);
            Assert.Equal(ParticipantGroup.All, request.Metadata.TargetGroup);

            AssertUpdateStrategy(request.ValueUpdate);
        }

        [Fact]
        public async Task Update_DifferentSyncObjName_SendUpdateSynchronizedObjectRequest()
        {
            const string conferenceId = "test";
            const string syncObjName = "differentName";

            // arrange
            var mediatorMock = new Mock<IMediator>();
            var capturedRequest =
                mediatorMock.CaptureRequest<UpdateSynchronizedObjectRequest<TestSyncObj>, TestSyncObj>();

            var initialValue = new TestSyncObj {TestVal = false};
            var syncObjProvider = new TestSyncObjProvider(mediatorMock.Object, initialValue, syncObjName);

            var newValue = new TestSyncObj {TestVal = true};

            // act
            await TriggerUpdate(syncObjProvider, conferenceId, newValue);

            // assert
            var request = capturedRequest.GetRequest();
            Assert.Equal(syncObjName, request.Metadata.Name);
        }

        [Fact]
        public async Task Update_DifferentSyncObjParticipantGroup_SendUpdateSynchronizedObjectRequest()
        {
            const string conferenceId = "test";
            const ParticipantGroup group = ParticipantGroup.Moderators;

            // arrange
            var mediatorMock = new Mock<IMediator>();
            var capturedRequest =
                mediatorMock.CaptureRequest<UpdateSynchronizedObjectRequest<TestSyncObj>, TestSyncObj>();

            var initialValue = new TestSyncObj {TestVal = false};
            var syncObjProvider = new TestSyncObjProvider(mediatorMock.Object, initialValue, null, group);

            var newValue = new TestSyncObj {TestVal = true};

            // act
            await TriggerUpdate(syncObjProvider, conferenceId, newValue);

            // assert
            var request = capturedRequest.GetRequest();
            Assert.Equal(group, request.Metadata.TargetGroup);
        }

        [Fact]
        public async Task GetInitialValue_DefaultValue_ReturnTheDefaultValue()
        {
            const string conferenceId = "test";

            // arrange
            var mediatorMock = new Mock<IMediator>();
            var initialValue = new TestSyncObj {TestVal = true};

            var syncObjProvider = new TestSyncObjProvider(mediatorMock.Object, initialValue);

            // act
            var receivedValue = await syncObjProvider.GetInitialValue(conferenceId);

            // assert
            Assert.NotNull(receivedValue);
            Assert.True(receivedValue.TestVal);
            mediatorMock.VerifyNoOtherCalls();
        }

        protected class TestSyncObj
        {
            public bool TestVal { get; set; }
        }

        protected class TestSyncObjProvider : SynchronizedObjectProvider<TestSyncObj>
        {
            private readonly TestSyncObj _initialValue;
            private readonly string? _name;
            private readonly ParticipantGroup? _targetGroup;

            public TestSyncObjProvider(IMediator mediator, TestSyncObj initialValue, string? name = null,
                ParticipantGroup? targetGroup = null) : base(mediator)
            {
                _initialValue = initialValue;
                _name = name;
                _targetGroup = targetGroup;
            }

            public override string Name => _name ?? base.Name;
            public override ParticipantGroup TargetGroup => _targetGroup ?? base.TargetGroup;

            public override ValueTask<TestSyncObj> GetInitialValue(string conferenceId)
            {
                return new(_initialValue);
            }
        }
    }
}
