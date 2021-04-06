using System.Threading.Tasks;

namespace Strive.Core.Services.Permissions
{
    /// <summary>
    ///     A permission stack defines the permissions for a participant and provides access to individual permissions
    /// </summary>
    public interface IPermissionStack
    {
        /// <summary>
        ///     Get a permission value
        /// </summary>
        /// <typeparam name="T">The type of the permission value</typeparam>
        /// <param name="descriptor">The descriptor of that permission</param>
        /// <returns>Return the current value of the permission if it exists, else return null</returns>
        ValueTask<T> GetPermissionValue<T>(PermissionDescriptor<T> descriptor);
    }
}
