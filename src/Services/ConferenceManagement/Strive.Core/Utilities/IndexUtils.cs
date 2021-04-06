using System;

namespace Strive.Core.Utilities
{
    public static class IndexUtils
    {
        /// <summary>
        ///     Translate negative indexes to actual indexes and enforce boundaries. If the start index is greater than the end
        ///     index, swap indexes, make sure that both indexes are greater or equal to zero and the end index is at most the last
        ///     index.
        ///     Please note that the start index may still be greater than the maximum index
        /// </summary>
        public static (int start, int end) TranslateStartEndIndex(int start, int end, int totalItems)
        {
            start = TranslateIndexAndEnforceBoundaries(totalItems, start);
            end = TranslateIndexAndEnforceBoundaries(totalItems, end);

            if (start > end) (start, end) = (end, start);

            end = Math.Min(end, totalItems - 1);
            return (start, end);
        }

        private static int TranslateIndexAndEnforceBoundaries(int length, int index)
        {
            if (index < 0) index = length + index;
            return Math.Max(index, 0);
        }
    }
}
