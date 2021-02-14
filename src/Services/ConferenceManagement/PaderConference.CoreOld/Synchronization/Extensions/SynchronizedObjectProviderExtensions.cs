using System.Threading.Tasks;
using PaderConference.Core.Services.Synchronization2;

namespace PaderConference.Core.Services.Synchronization.Extensions
{
    public static class SynchronizedObjectProviderExtensions
    {
        public static async ValueTask<T> UpdateWithInitialValue<T>(
            this ISynchronizedObjectProvider<T> synchronizedObjectProvider, string conferenceId)
        {
            var value = await synchronizedObjectProvider.GetInitialValue(conferenceId);
            return await synchronizedObjectProvider.Update(conferenceId, value);
        }
    }
}
