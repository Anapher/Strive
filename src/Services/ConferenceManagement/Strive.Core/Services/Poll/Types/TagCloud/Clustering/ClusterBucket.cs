using System.Collections.Generic;

namespace Strive.Core.Services.Poll.Types.TagCloud.Clustering
{
    public readonly struct ClusterBucket<T>
    {
        public ClusterBucket(List<T> entities)
        {
            Entities = entities;
        }

        public List<T> Entities { get; }
    }
}
