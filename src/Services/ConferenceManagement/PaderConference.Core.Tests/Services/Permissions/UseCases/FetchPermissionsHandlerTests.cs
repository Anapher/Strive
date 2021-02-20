using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Services.Permissions;
using PaderConference.Core.Services.Permissions.Requests;
using PaderConference.Core.Services.Permissions.UseCases;
using Xunit;

namespace PaderConference.Core.Tests.Services.Permissions.UseCases
{
    public class FetchPermissionsHandlerTests
    {
        private readonly Mock<IPermissionLayersAggregator> _aggregator = new();
        private const string ConferenceId = "123";
        private const string ParticipantId = "asdf";

        private FetchPermissionsHandler Create()
        {
            return new(_aggregator.Object);
        }

        [Fact]
        public async Task Handle_FetchPermissions_ReturnPermission()
        {
            // arrange
            var permissionLayer = new PermissionLayer(23, "asd", new Dictionary<string, JValue>());

            _aggregator.Setup(x => x.FetchParticipantPermissionLayers(ConferenceId, ParticipantId))
                .ReturnsAsync(new List<PermissionLayer> {permissionLayer});

            var handler = Create();

            // act
            var result = await handler.Handle(new FetchPermissionsRequest(ParticipantId, ConferenceId),
                CancellationToken.None);

            // assert
            var actualLayer = Assert.Single(result.Layers);
            Assert.Same(permissionLayer, actualLayer);
        }
    }
}
