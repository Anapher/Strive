using System.Threading.Tasks;
using Strive.Core.Interfaces;
using Strive.Core.Services.Media;

namespace Strive.Hubs
{
    public interface ISfuConnectionHub
    {
        Task<SuccessOrError<SfuConnectionInfo>> FetchSfuConnectionInfo();
    }
}
