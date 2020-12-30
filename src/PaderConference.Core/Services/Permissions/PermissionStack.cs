using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A simple permission stack that is based on a list of permission layers (cached)
    /// </summary>
    public class PermissionStack : IPermissionStack
    {
        private readonly IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> _stack;

        /// <summary>
        ///     Initialize a new instance of <see cref="PermissionStack" />
        /// </summary>
        /// <param name="stack">
        ///     The permission layers. The layers with greater indexes are the layers with a higher order, see
        ///     <see cref="PermissionLayer.Order" /> for more information.
        /// </param>
        public PermissionStack(IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> stack)
        {
            _stack = stack;
        }

        /// <summary>
        ///     Flatten the permission stack so only the actually enforced permissions are returned
        /// </summary>
        /// <returns>Return a dictionary with permission keys and their value</returns>
        public Dictionary<string, JsonElement> Flatten()
        {
            var result = new Dictionary<string, JsonElement>();

            foreach (var key in _stack.SelectMany(x => x.Keys).Distinct())
            foreach (var layer in _stack.Reverse())
                if (layer.TryGetValue(key, out var value))
                {
                    result[key] = value;
                    break;
                }

            return result;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async ValueTask<T> GetPermission<T>(PermissionDescriptor<T> descriptor)
        {
            foreach (var layer in _stack.Reverse())
            {
                if (layer.TryGetValue(descriptor.Key, out var value))
                    switch (descriptor.Type)
                    {
                        case PermissionValueType.Boolean:
                            return (T) (object) value.GetBoolean();
                        case PermissionValueType.Integer:
                            return (T) (object) value.GetInt32();
                        case PermissionValueType.Decimal:
                            return (T) (object) value.GetDouble();
                        case PermissionValueType.Text:
                            var val = value.GetString();
                            if (val == null)
                                throw new NullReferenceException("The permission descriptor has a null value.");

                            return (T) (object) val;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(descriptor),
                                $"Invalid descriptor type. \"{descriptor.Type}\" is not supported.");
                    }
            }

            return (T) descriptor.DefaultValue;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
