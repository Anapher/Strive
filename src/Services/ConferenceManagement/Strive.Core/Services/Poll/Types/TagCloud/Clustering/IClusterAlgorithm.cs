using System;
using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.TagCloud.Clustering
{
    public interface IClusterAlgorithm
    {
        IEnumerable<ClusterBucket<T>> BuildCluster<T, TValue>(IEnumerable<T> data, Func<T, TValue> valueSelector,
            IDistanceAlgorithm<TValue> distanceAlgorithm, double threshold);
    }
}
