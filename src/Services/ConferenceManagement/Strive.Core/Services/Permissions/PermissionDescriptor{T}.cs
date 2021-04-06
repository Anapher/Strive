namespace Strive.Core.Services.Permissions
{
    /// <summary>
    ///     A generic permission descriptor used to describe a permission
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PermissionDescriptor<T> : PermissionDescriptor
    {
        public PermissionDescriptor(string key, T? defaultValue = default) : base(key, TypeMapReverse[typeof(T)],
            defaultValue)
        {
        }
    }
}
