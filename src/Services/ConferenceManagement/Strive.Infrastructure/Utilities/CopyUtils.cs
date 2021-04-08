using Newtonsoft.Json;

namespace Strive.Infrastructure.Utilities
{
    public static class CopyUtils
    {
        public static T DeepClone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
