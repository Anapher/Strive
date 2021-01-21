using System;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Utilities
{
    public class DelegateAsyncDisposable : IAsyncDisposable
    {
        private readonly Func<Task> _action;

        public DelegateAsyncDisposable(Func<Task> action)
        {
            _action = action;
        }

        public async ValueTask DisposeAsync()
        {
            await _action();
        }
    }
}
