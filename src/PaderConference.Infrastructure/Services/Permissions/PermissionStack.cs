using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public interface IPermissionStack
    {
        T GetPermission<T>(PermissionDescriptor<T> descriptor);
    }

    public class PermissionStack : IPermissionStack
    {
        private readonly IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> _stack;

        public PermissionStack(IReadOnlyList<IReadOnlyDictionary<string, JsonElement>> stack)
        {
            _stack = stack;
        }

        public T GetPermission<T>(PermissionDescriptor<T> descriptor)
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
    }
}