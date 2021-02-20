using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Extensions;
using PaderConference.Core.Services.Permissions;
using PaderConference.Tests.Utils;
using Xunit;

namespace PaderConference.Core.Tests.Services.Permissions
{
    public class PermissionLayersAggregatorTests
    {
        private const string ConferenceId = "123";
        private const string ParticipantId = "45";

        private readonly KeyValuePair<string, JValue> _testPermission = new("test", (JValue) JToken.FromObject("test"));

        [Fact]
        public async Task FetchAggregatedPermissions_NoProviders_Empty()
        {
            // arrange
            var providers = Enumerable.Empty<IPermissionLayerProvider>();
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchAggregatedPermissions(ConferenceId, ParticipantId);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchAggregatedPermissions_SingleProvider_ReturnPermissions()
        {
            // arrange
            var permissions = new Dictionary<string, JValue>(_testPermission.Yield());

            var providerMock = new Mock<IPermissionLayerProvider>();
            providerMock.Setup(x => x.FetchPermissionsForParticipant(ConferenceId, ParticipantId))
                .ReturnsAsync(new PermissionLayer(1, "Test", permissions).Yield());

            var providers = new[] {providerMock.Object};
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchAggregatedPermissions(ConferenceId, ParticipantId);

            // assert
            AssertHelper.AssertScrambledEquals(permissions, result);
        }

        [Fact]
        public async Task FetchParticipantPermissionLayers_SingleProvider_ReturnLayer()
        {
            // arrange
            var permissions = new Dictionary<string, JValue>(_testPermission.Yield());
            var layer = new PermissionLayer(1, "Test", permissions);

            var providerMock = new Mock<IPermissionLayerProvider>();
            providerMock.Setup(x => x.FetchPermissionsForParticipant(ConferenceId, ParticipantId))
                .ReturnsAsync(layer.Yield());

            var providers = new[] {providerMock.Object};
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchParticipantPermissionLayers(ConferenceId, ParticipantId);

            // assert
            var actualLayer = Assert.Single(result);
            Assert.Same(layer, actualLayer);
        }
    }
}
