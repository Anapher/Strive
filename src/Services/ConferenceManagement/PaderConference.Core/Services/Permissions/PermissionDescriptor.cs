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
        protected static readonly IReadOnlyDictionary<PermissionValueType, Type> TypeMap =
            new Dictionary<PermissionValueType, Type>
            {
                {PermissionValueType.Boolean, typeof(bool)},
                {PermissionValueType.Integer, typeof(int)},
                {PermissionValueType.Decimal, typeof(double)},
                {PermissionValueType.Text, typeof(string)},
            };

        protected static readonly IReadOnlyDictionary<Type, PermissionValueType> TypeMapReverse =
            TypeMap.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        ///     Initialize a new instance of <see cref="PermissionDescriptor" />
        /// </summary>
        /// <param name="key">The key of the permission</param>
        /// <param name="type">The type of the value of the permission</param>
        /// <param name="defaultValue">The default value of the permission</param>
        public PermissionDescriptor(string key, PermissionValueType type, object? defaultValue = null)
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
                    case PermissionValueType.Boolean:
                        DefaultValue = false;
                        break;
                    case PermissionValueType.Integer:
                        DefaultValue = 0;
                        break;
                    case PermissionValueType.Decimal:
                        DefaultValue = 0.0;
                        break;
                    case PermissionValueType.Text:
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
        public PermissionValueType Type { get; }

        /// <summary>
        ///     The default value of the permission
        /// </summary>
        public object DefaultValue { get; }
    }
}
