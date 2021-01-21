namespace PaderConference.Core.Services.ConferenceControl
{
    public record OptionDescriptor<T>
    {
        public OptionDescriptor(string key, T? defaultValue = default)
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public string Key { get; }

        public T? DefaultValue { get; }
    }
}
