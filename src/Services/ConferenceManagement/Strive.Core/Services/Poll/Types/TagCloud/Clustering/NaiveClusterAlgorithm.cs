// https://en.wikipedia.org/wiki/Single-linkage_clustering

using System;
using System.Collections.Generic;
using System.Linq;

namespace Strive.Core.Services.Poll.Types.TagCloud.Clustering
{
    public class NaiveClusterAlgorithm : IClusterAlgorithm
    {
        public IEnumerable<ClusterBucket<T>> BuildCluster<T, TValue>(IEnumerable<T> data, Func<T, TValue> valueSelector,
            IDistanceAlgorithm<TValue> distanceAlgorithm, double threshold)
        {
            var clusterBuckets = data.Select(x => new ClusterBucket<T>(new List<T> {x})).ToArray();
            var entriesMatrix = new double?[clusterBuckets.Length, clusterBuckets.Length];

            for (var i = 0; i < clusterBuckets.Length; i++)
            {
                for (var j = 0; j < clusterBuckets.Length; j++)
                {
                    if (i == j)
                    {
                        // diagonal is zero
                        entriesMatrix[i, j] = 0;
                    }
                    else
                    {
                        // already set
                        if (entriesMatrix[i, j] != null) continue;

                        var distance = distanceAlgorithm.CalculateDistance(
                            valueSelector(clusterBuckets[i].Entities.Single()),
                            valueSelector(clusterBuckets[j].Entities.Single()));

                        // as distance is symmetrical
                        entriesMatrix[i, j] = distance;
                        entriesMatrix[j, i] = distance;
                    }
                }
            }

            while (TryMergeSimilarBuckets(clusterBuckets, entriesMatrix, threshold))
            {
            }

            return clusterBuckets.Where(x => x.Entities.Any());
        }

        private static bool TryMergeSimilarBuckets<T>(ClusterBucket<T>[] clusterBuckets, double?[,] entriesMatrix,
            double threshold)
        {
            for (var i = 0; i < clusterBuckets.Length; i++)
            {
                for (var j = 0; j < clusterBuckets.Length; j++)
                {
                    if (i == j) continue;
                    if (entriesMatrix[i, j] == null) continue;

                    if (entriesMatrix[i, j] < threshold)
                    {
                        // merge cluster buckets
                        clusterBuckets[i].Entities.AddRange(clusterBuckets[j].Entities);
                        clusterBuckets[j].Entities.Clear();

                        // update similarities for new bucket, use min
                        for (var k = 0; k < clusterBuckets.Length; k++)
                        {
                            var updated = NullableMin(entriesMatrix[i, k], entriesMatrix[j, k]);
                            entriesMatrix[i, k] = updated;
                            entriesMatrix[k, i] = updated;

                            // set distance of the removed bucket (j) null
                            entriesMatrix[j, k] = null;
                            entriesMatrix[k, j] = null;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static double? NullableMin(double? x, double? y)
        {
            if (x == null || y == null) return null;
            return Math.Min(x.Value, y.Value);
        }
    }
}
