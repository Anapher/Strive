using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaderConference.Core.Services.Permissions
{
    public interface IPermissionStack
    {
        ValueTask<T> GetPermission<T>(PermissionDescriptor<T> descriptor);
    }

    public class PermissionStack : IPermissionStack
    {
        private readonly IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> _stack;

        public PermissionStack(IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> stack)
        {
            _stack = stack;
        }

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
                if (layer.TryGetValue(descriptor.Key, out var value))
                    switch (descriptor.Type)
                    {
                        case PermissionType.Boolean:
                            return (T) (object) value.GetBoolean();
                        case PermissionType.Integer:
                            return (T) (object) value.GetInt32();
                        case PermissionType.Decimal:
                            return (T) (object) value.GetDouble();
                        case PermissionType.Text:
                            return (T) (object) value.GetString();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

            return (T) descriptor.DefaultValue;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
