using System.Threading.Tasks;
using PaderConference.Core.Interfaces;
using PaderConference.Core.Services.Media;

namespace PaderConference.Hubs
{
    public interface ISfuConnectionHub
    {
        Task<SuccessOrError<SfuConnectionInfo>> FetchSfuConnectionInfo();
    }
}
