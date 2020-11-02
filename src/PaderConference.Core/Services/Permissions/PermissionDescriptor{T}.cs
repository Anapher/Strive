namespace PaderConference.Core.Services.Permissions
{
    public class PermissionDescriptor<T> : PermissionDescriptor
    {
        public PermissionDescriptor(string key, T defaultValue = default) : base(key, TypeMapReverse[typeof(T)],
            defaultValue)
        {
        }
    }
}
