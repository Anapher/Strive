using System;
using Mongo2Go.Helper;

namespace PaderConference.IntegrationTests._Helpers
{
    public sealed class Mongo2GoPortPool : IPortPool
    {
        private readonly object _lock = new();
        private int _startPort;

        private Mongo2GoPortPool()
        {
            _startPort = new Random().Next(27020, 27999);
        }

        public static Mongo2GoPortPool Instance { get; } = new();

        public int GetNextOpenPort()
        {
            lock (_lock)
            {
                var openPort = PortWatcherFactory.CreatePortWatcher().FindOpenPort(_startPort);
                _startPort = openPort + 1;
                return openPort;
            }
        }
    }
}
