using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace PaderConference.Hubs.Media
{
    public class AsyncObjectQueue<T> where T : struct
    {
        private readonly AsyncManualResetEvent _asyncManualResetEvent = new AsyncManualResetEvent(false);
        private readonly object _lock = new object();
        private ObjInstance<T> _current;
        private int _instanceCounter = 1;
        private int _receivedInstanceId = 1;

        public AsyncObjectQueue(T defaultValue)
        {
            _current = new ObjInstance<T>(defaultValue, _instanceCounter);
        }

        public void Set(T obj)
        {
            var instanceId = Interlocked.Increment(ref _instanceCounter);
            _current = new ObjInstance<T>(obj, instanceId);
            _asyncManualResetEvent.Set();
        }

        public async ValueTask<T> Receive()
        {
            var current = _current;
            var received = _receivedInstanceId;

            if (current.InstanceId > received)
            {
                var result = Interlocked.CompareExchange(ref _receivedInstanceId, current.InstanceId, received);

                if (result == current.InstanceId)
                    return current.Obj;

                // receive was completed somewhere else
                return await Receive();
            }

            var waitTask = _asyncManualResetEvent.WaitAsync();

            if (_current.InstanceId != current.InstanceId)
                return await Receive();

            await waitTask;

            return await Receive();
        }

        public readonly struct ObjInstance<T>
        {
            public ObjInstance(T obj, int instanceId)
            {
                Obj = obj;
                InstanceId = instanceId;
            }

            public T Obj { get; }
            public int InstanceId { get; }
        }
    }
}