using System.Collections.Generic;
using System.Text.Json;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     Define a new permission layer that will override permissions defined in layers below
    /// </summary>
    public class PermissionLayer
    {
        public PermissionLayer(int order, string name, IReadOnlyDictionary<string, JsonElement> permissions)
        {
            Order = order;
            Name = name;
            Permissions = permissions;
        }

        /// <summary>
        ///     The order defines where the permission layer will be placed. Please note that this is proportional to the
        ///     importance of this layer, so a higher order will mean that it may override more other layers
        /// </summary>
        public int Order { get; }

        /// <summary>
        ///     The name of this permission layer. Please note that this is for displaying purposes only and serves no actual use.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The permissions defined in this layer
        /// </summary>
        public IReadOnlyDictionary<string, JsonElement> Permissions { get; }
    }
}
