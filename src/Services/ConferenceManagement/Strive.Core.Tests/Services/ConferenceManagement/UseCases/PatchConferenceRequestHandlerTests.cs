using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Strive.Core.Domain.Entities;
using Strive.Core.Errors;
using Strive.Core.Interfaces.Gateways;
using Strive.Core.Services.ConferenceManagement;
using Strive.Core.Services.ConferenceManagement.Gateways;
using Strive.Core.Services.ConferenceManagement.Requests;
using Strive.Core.Services.ConferenceManagement.UseCases;
using Strive.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Strive.Core.Tests.Services.ConferenceManagement.UseCases
{
    public class PatchConferenceRequestHandlerTests
    {
        private const string ConferenceId = "123";

        private readonly Mock<IConferenceRepo> _repo = new();
        private readonly ConcurrencyOptions _options = new();
        private readonly ILogger<PatchConferenceRequestHandler> _logger;

        public PatchConferenceRequestHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _logger = testOutputHelper.CreateLogger<PatchConferenceRequestHandler>();
        }

        private PatchConferenceRequestHandler Create()
        {
            return new(_repo.Object, new OptionsWrapper<ConcurrencyOptions>(_options), _logger);
        }

        private void SetupConference(ConferenceData conferenceData)
        {
            _repo.Setup(x => x.FindById(ConferenceId)).ReturnsAsync(new Conference(ConferenceId)
            {
                Permissions = conferenceData.Permissions, Configuration = conferenceData.Configuration,
            });
        }

        private static JsonPatchDocument<ConferenceData> GetValidPatch()
        {
            return new();
        }

        private static ConferenceData GetValidConferenceData()
        {
            var conferenceData = new ConferenceData();
            conferenceData.Configuration.Moderators = new List<string> {"test"};
            new ConferenceDataValidator().ValidateAndThrow(conferenceData);

            return conferenceData;
        }

        [Fact]
        public async Task Handle_ConferenceDoesNotExist_ReturnError()
        {
            // arrange
            var patch = GetValidPatch();
            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, patch), CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ConferenceError.ConferenceNotFound, result.Error);
        }

        [Fact]
        public async Task Handle_InvalidPatch_ReturnError()
        {
            // arrange
            var invalidPatch = new JsonPatchDocument<ConferenceData>();
            invalidPatch.Operations.Add(new Operation<ConferenceData>("add", "invalid-patch", null, "yo"));

            SetupConference(GetValidConferenceData());

            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, invalidPatch),
                CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ErrorCode.FieldValidation.ToString(), result.Error?.Code);
        }

        [Fact]
        public async Task Handle_PatchCreatesInvalidConference_ReturnError()
        {
            // arrange
            var invalidPatch = new JsonPatchDocument<ConferenceData>();
            invalidPatch.Remove(x => x.Configuration.Moderators, 0);

            SetupConference(GetValidConferenceData());

            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, invalidPatch),
                CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ErrorCode.FieldValidation.ToString(), result.Error?.Code);
        }

        [Fact]
        public async Task Handle_ValidPatch_IsSavedInRepository()
        {
            const string newModerator = "test123";

            // arrange
            var invalidPatch = new JsonPatchDocument<ConferenceData>();
            invalidPatch.Add(x => x.Configuration.Moderators, newModerator);

            _repo.Setup(x => x.Update(It.IsAny<Conference>())).ReturnsAsync(OptimisticUpdateResult.Ok);

            SetupConference(GetValidConferenceData());

            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, invalidPatch),
                CancellationToken.None);

            // assert
            Assert.True(result.Success);

            _repo.Verify(
                x => x.Update(
                    It.Is<Conference>(conference => conference.Configuration.Moderators.Contains(newModerator))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateResultDeleted_OperationIsCancelledReturnError()
        {
            const string newModerator = "test123";

            // arrange
            var invalidPatch = new JsonPatchDocument<ConferenceData>();
            invalidPatch.Add(x => x.Configuration.Moderators, newModerator);

            _repo.Setup(x => x.Update(It.IsAny<Conference>())).ReturnsAsync(OptimisticUpdateResult.DeletedException);

            SetupConference(GetValidConferenceData());

            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, invalidPatch),
                CancellationToken.None);

            // assert
            Assert.False(result.Success);
            Assert.Equal(ConferenceError.ConferenceNotFound, result.Error);

            _repo.Verify(x => x.Update(It.IsAny<Conference>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ConcurrencyExceptionOnFirst3Requests_RetryUntilNoException()
        {
            const string newModerator = "test123";

            // arrange
            var invalidPatch = new JsonPatchDocument<ConferenceData>();
            invalidPatch.Add(x => x.Configuration.Moderators, newModerator);

            _repo.SetupSequence(x => x.Update(It.IsAny<Conference>()))
                .ReturnsAsync(OptimisticUpdateResult.ConcurrencyException)
                .ReturnsAsync(OptimisticUpdateResult.ConcurrencyException)
                .ReturnsAsync(OptimisticUpdateResult.ConcurrencyException).ReturnsAsync(OptimisticUpdateResult.Ok);

            SetupConference(GetValidConferenceData());

            var useCase = Create();

            // act
            var result = await useCase.Handle(new PatchConferenceRequest(ConferenceId, invalidPatch),
                CancellationToken.None);

            // assert
            Assert.True(result.Success);

            _repo.Verify(x => x.Update(It.IsAny<Conference>()), Times.Exactly(4));
        }
    }
}
