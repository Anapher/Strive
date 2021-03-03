using System;

namespace PaderConference.Config
{
    public class RabbitMqOptions
    {
        public bool UseInMemory { get; set; }

        public RabbitMqHostOptions? RabbitMq { get; set; }
    }

    public class RabbitMqHostOptions
    {
        public Uri? Host { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}
