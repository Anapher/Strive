using System;
using System.Collections.Immutable;

namespace PaderConference.Core.Services.BreakoutRooms.Naming
{
    public class NatoRoomNamingStrategy : IRoomNamingStrategy
    {
        private static readonly IImmutableList<string> NatoAlpha = new[]
        {
            "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliet", "Kilo",
            "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor",
            "Whiskey", "X-ray", "Yankee", "Zulu",
        }.ToImmutableList();

        public string GetName(int index)
        {
            return NatoAlpha[index % 26] + (index > 25 ? $" #{Math.Floor(index / 26d) + 1}" : string.Empty);
        }

        public int ParseIndex(string name)
        {
            var splits = name.Split(' ');
            var factor = splits.Length == 1 ? 0 : int.Parse(splits[1].TrimStart('#')) - 1;
            var position = NatoAlpha.IndexOf(splits[0]);

            return position + NatoAlpha.Count * factor;
        }
    }
}
