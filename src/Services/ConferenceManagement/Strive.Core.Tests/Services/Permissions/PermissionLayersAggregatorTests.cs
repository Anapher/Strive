using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Strive.Core.Extensions;
using Strive.Core.Services;
using Strive.Core.Services.Permissions;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Permissions
{
    public class PermissionLayersAggregatorTests
    {
        private const string ConferenceId = "123";
        private const string ParticipantId = "45";

        private static readonly Participant Participant = new(ConferenceId, ParticipantId);

        private readonly KeyValuePair<string, JValue> _testPermission = new("test", (JValue) JToken.FromObject("test"));

        [Fact]
        public async Task FetchAggregatedPermissions_NoProviders_Empty()
        {
            // arrange
            var providers = Enumerable.Empty<IPermissionLayerProvider>();
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchAggregatedPermissions(Participant);

            // assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task FetchAggregatedPermissions_SingleProvider_ReturnPermissions()
        {
            // arrange
            var permissions = new Dictionary<string, JValue>(_testPermission.Yield());

            var providerMock = new Mock<IPermissionLayerProvider>();
            providerMock.Setup(x => x.FetchPermissionsForParticipant(Participant))
                .ReturnsAsync(new PermissionLayer(1, "Test", permissions).Yield());

            var providers = new[] {providerMock.Object};
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchAggregatedPermissions(Participant);

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
            providerMock.Setup(x => x.FetchPermissionsForParticipant(Participant)).ReturnsAsync(layer.Yield());

            var providers = new[] {providerMock.Object};
            var aggregator = new PermissionLayersAggregator(providers);

            // act
            var result = await aggregator.FetchParticipantPermissionLayers(Participant);

            // assert
            var actualLayer = Assert.Single(result);
            Assert.Same(layer, actualLayer);
        }
    }
}
