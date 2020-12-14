using System;
using System.Collections.Generic;
using System.Linq;

namespace PaderConference.Core.Services.Permissions
{
    /// <summary>
    ///     A description for a permission value
    /// </summary>
    public class PermissionDescriptor
    {
        protected static readonly IReadOnlyDictionary<PermissionType, Type> TypeMap =
            new Dictionary<PermissionType, Type>
            {
                {PermissionType.Boolean, typeof(bool)},
                {PermissionType.Integer, typeof(int)},
                {PermissionType.Decimal, typeof(double)},
                {PermissionType.Text, typeof(string)},
            };

        protected static readonly IReadOnlyDictionary<Type, PermissionType> TypeMapReverse =
            TypeMap.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        ///     Initialize a new instance of <see cref="PermissionDescriptor" />
        /// </summary>
        /// <param name="key">The key of the permission</param>
        /// <param name="type">The type of the value of the permission</param>
        /// <param name="defaultValue">The default value of the permission</param>
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

        /// <summary>
        ///     The key of the permission
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     The type of the value of the permission
        /// </summary>
        public PermissionType Type { get; }

        /// <summary>
        ///     The default value of the permission
        /// </summary>
        public object DefaultValue { get; }
    }
}
