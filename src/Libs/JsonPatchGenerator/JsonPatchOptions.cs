using System.Collections.Generic;
using JsonPatchGenerator.Handlers;

namespace JsonPatchGenerator
{
    public class JsonPatchOptions
    {
        public List<IPatchTypeHandler> Handlers { get; set; } = new()
            {new ArrayPatchTypeHandler(), new ObjectPatchTypeHandler(), new ReplaceValuePatchTypeHandler()};
    }
}