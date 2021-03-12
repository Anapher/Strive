using System.Threading;
using System.Threading.Tasks;
using Moq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Synchronization;
using PaderConference.Core.Services.Synchronization.Gateways;
using PaderConference.Core.Services.Synchronization.Requests;
using PaderConference.Core.Services.Synchronization.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.Synchronization.UseCases
{
    public class FetchSynchronizedObjectUseCaseTests
    {
        private const string SyncObjId = "test";
        private const string ConferenceId = "conferenceId";

        private readonly Mock<ISynchronizedObjectProvider> _provider = new();
        private readonly Mock<ISynchronizedObjectRepository> _repository = new();

        private FetchSynchronizedObjectUseCase Create()
        {
            _provider.SetupGet(x => x.Id).Returns(SyncObjId);
            return new(_repository.Object, _provider.Object.Yield());
        }

        private void SetupSyncObjIsStored(object value)
        {
            _repository.Setup(x => x.Get(ConferenceId, SyncObjId, value.GetType())).ReturnsAsync(value);
        }

        [Fact]
        public async Task Handle_ValueStoredInRepository_ReturnValue()
        {
            const string value = "hello world";

            // arrange
            _provider.SetupGet(x => x.Type).Returns(value.GetType);

            SetupSyncObjIsStored(value);

            var useCase = Create();
            var request = new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task Handle_ValueNotStored_FetchValueFromProvider()
        {
            const string value = "hello world";

            // arrange
            _provider.SetupGet(x => x.Type).Returns(value.GetType);
            _provider.Setup(x => x.FetchValue(ConferenceId, It.IsAny<SynchronizedObjectId>())).ReturnsAsync(value);

            var useCase = Create();
            var request = new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));

            // act
            var result = await useCase.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task Handle_ValueNotStored_StoreValueInRepository()
        {
            const string value = "hello world";

            // arrange
            _provider.SetupGet(x => x.Type).Returns(value.GetType);
            _provider.Setup(x => x.FetchValue(ConferenceId, It.IsAny<SynchronizedObjectId>())).ReturnsAsync(value);

            var useCase = Create();
            var request = new FetchSynchronizedObjectRequest(ConferenceId, SynchronizedObjectId.Parse(SyncObjId));

            // act
            await useCase.Handle(request, CancellationToken.None);

            // assert
            _repository.Verify(x => x.Create(ConferenceId, SyncObjId, value, value.GetType()));
        }
    }
}
