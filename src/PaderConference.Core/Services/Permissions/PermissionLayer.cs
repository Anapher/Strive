using System.Collections.Generic;
using System.Text.Json;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     Define a new permission layer that will override permissions defined in layers below
    /// </summary>
    public class PermissionLayer
    {
        // some permission orders
        public const int PERMISSION_LAYER_CONFERENCE_DEFAULT = 10;
        public const int PERMISSION_LAYER_CONFERENCE = 11;
        public const int PERMISSION_LAYER_BREAKOUTROOM_DEFAULT = 20;
        public const int PERMISSION_LAYER_BREAKOUTROOM = 21;
        public const int PERMISSION_LAYER_MODERATOR_DEFAULT = 30;
        public const int PERMISSION_LAYER_MODERATOR = 31;
        public const int PERMISSION_LAYER_TEMPORARY = 100;

        public PermissionLayer(int order, IReadOnlyDictionary<string, JsonElement> permissions)
        {
            Order = order;
            Permissions = permissions;
        }

        /// <summary>
        ///     The order defines where the permission layer will be placed. Please note that this is proportional to the
        ///     importance of this layer, so a higher order will mean that it may override more other layers
        /// </summary>
        public int Order { get; }

        /// <summary>
        ///     The permissions defined in this layer
        /// </summary>
        public IReadOnlyDictionary<string, JsonElement> Permissions { get; }
    }
}
