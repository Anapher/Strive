using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PaderConference.Core.Extensions;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A simple permission stack that is based on a list of permission layers (cached)
    /// </summary>
    public class CachedPermissionStack : IPermissionStack
    {
        private readonly IReadOnlyList<IReadOnlyDictionary<string, JValue>> _stack;

        /// <summary>
        ///     Initialize a new instance of <see cref="CachedPermissionStack" />
        /// </summary>
        /// <param name="stack">
        ///     The permission layers. The layers with greater indexes are the layers with a higher order, see
        ///     <see cref="PermissionLayer.Order" /> for more information.
        /// </param>
        public CachedPermissionStack(IReadOnlyList<IReadOnlyDictionary<string, JValue>> stack)
        {
            _stack = stack;
        }

        /// <summary>
        ///     Initialize a new instance of <see cref="CachedPermissionStack" />
        /// </summary>
        /// <param name="singleLayer">The single layer of the stack</param>
        public CachedPermissionStack(IReadOnlyDictionary<string, JValue> singleLayer)
        {
            _stack = singleLayer.Yield().ToList();
        }

        /// <summary>
        ///     Flatten the permission stack so only the actually enforced permissions are returned
        /// </summary>
        /// <returns>Return a dictionary with permission keys and their value</returns>
        public Dictionary<string, JValue> Flatten()
        {
            var result = new Dictionary<string, JValue>();

            foreach (var key in _stack.SelectMany(x => x.Keys).Distinct())
            foreach (var layer in _stack.Reverse())
                if (layer.TryGetValue(key, out var value))
                {
                    result[key] = value;
                    break;
                }

            return result;
        }

        public async ValueTask<T> GetPermissionValue<T>(PermissionDescriptor<T> descriptor)
        {
            foreach (var layer in _stack.Reverse())
            {
                if (layer.TryGetValue(descriptor.Key, out var value))
                    switch (descriptor.Type)
                    {
                        case PermissionValueType.Boolean:
                            return (T) (object) value.ToObject<bool>();
                        case PermissionValueType.Integer:
                            return (T) (object) value.ToObject<int>();
                        case PermissionValueType.Decimal:
                            return (T) (object) value.ToObject<double>();
                        case PermissionValueType.Text:
                            var val = value.ToObject<string?>();
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
    }
}
