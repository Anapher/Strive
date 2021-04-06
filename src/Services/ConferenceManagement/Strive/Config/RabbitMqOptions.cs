namespace Strive.Config
{
    public class RabbitMqOptions
    {
        public bool UseInMemory { get; set; }

        public RabbitMqHostOptions? RabbitMq { get; set; }
    }

    public class RabbitMqHostOptions
    {
        public string? Host { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}
