using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Infrastructure.Services.Permissions
{
    public class PermissionDescriptor
    {
        protected static readonly IReadOnlyDictionary<PermissionType, Type> TypeMap =
            new Dictionary<PermissionType, Type>
            {
                {
                    PermissionType.Boolean, typeof(bool)
                },
                {
                    PermissionType.Integer, typeof(int)
                },
                {
                    PermissionType.Decimal, typeof(double)
                },
                {
                    PermissionType.Text, typeof(string)
                }
            };

        protected static readonly IReadOnlyDictionary<Type, PermissionType> TypeMapReverse =
            TypeMap.ToDictionary(x => x.Value, x => x.Key);

        public PermissionDescriptor(string key, PermissionType type, object? defaultValue = null)
        {
            Key = key;
            Type = type;


            if (defaultValue != null)
            {
                if (defaultValue.GetType() != TypeMap[type])
                    throw new ArgumentException("Invalid type for default value");
                DefaultValue = defaultValue;
            }
            else
            {
                switch (type)
                {
                    case PermissionType.Boolean:
                        DefaultValue = false;
                        break;
                    case PermissionType.Integer:
                        DefaultValue = 0;
                        break;
                    case PermissionType.Decimal:
                        DefaultValue = 0.0;
                        break;
                    case PermissionType.Text:
                        DefaultValue = "";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public string Key { get; }

        public object DefaultValue { get; }

        public PermissionType Type { get; }
    }
}