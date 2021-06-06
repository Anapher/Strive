using System;
using System.Collections.Generic;
using System.Linq;
using Strive.Core.Services.Poll.Types.TagCloud.Clustering;

namespace Strive.Core.Services.Poll.Types.TagCloud
{
    public static class TagCloudGrouper
    {
        public static IReadOnlyList<IReadOnlyList<TagCloudTag>> GroupAnswers(TagCloudClusterMode mode,
            IEnumerable<TagCloudTag> answers)
        {
            switch (mode)
            {
                case TagCloudClusterMode.CaseInsensitive:
                    return answers.GroupBy(x => x.Tag, StringComparer.OrdinalIgnoreCase)
                        .Select(x => (IReadOnlyList<TagCloudTag>) x.ToList()).ToList();
                case TagCloudClusterMode.Fuzzy:
                    var distanceAlgo = new NormalizedLevenshteinDistance();
                    var clusterAlgo = new NaiveClusterAlgorithm();

                    var cloud = clusterAlgo.BuildCluster(answers, x => x.Tag.ToUpper(), distanceAlgo,
                        NormalizedLevenshteinDistance.Threshold);

                    return cloud.Select(x => (IReadOnlyList<TagCloudTag>) x.Entities).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
