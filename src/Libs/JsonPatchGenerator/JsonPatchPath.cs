using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonPatchGenerator
{
    public class JsonPatchPath
    {
        private readonly IReadOnlyList<string> _segments;

        public JsonPatchPath(IReadOnlyList<string> segments)
        {
            _segments = segments;
        }

        public static JsonPatchPath Root { get; } = new(Array.Empty<string>());

        public JsonPatchPath AddSegment(string s)
        {
            var newSegments = _segments.ToList();
            newSegments.Add(s);

            return new JsonPatchPath(newSegments);
        }

        public JsonPatchPath AtIndex(int i)
        {
            return AddSegment(i.ToString());
        }

        public override string ToString()
        {
            if (_segments.Count == 0) return string.Empty;

            var result = "/";
            result += string.Join("/", _segments);

            return result;
        }
    }
}