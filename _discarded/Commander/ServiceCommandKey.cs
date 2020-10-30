using System;

namespace PaderConference.Infrastructure.Commander
{
    public struct ServiceCommandKey
    {
        public ServiceCommandKey(string service, string method)
        {
            Service = service;
            Method = method;
        }

        public string Service { get; }

        public string Method { get; }

        public bool Equals(ServiceCommandKey other)
        {
            return Service == other.Service && Method == other.Method;
        }

        public override bool Equals(object? obj)
        {
            return obj is ServiceCommandKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Service, Method);
        }
    }
}
