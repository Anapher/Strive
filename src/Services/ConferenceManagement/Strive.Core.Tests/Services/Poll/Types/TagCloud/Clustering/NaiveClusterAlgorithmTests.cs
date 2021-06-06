using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Strive.Core.Services.Poll.Types.TagCloud.Clustering;
using Strive.Tests.Utils;
using Xunit;

namespace Strive.Core.Tests.Services.Poll.Types.TagCloud.Clustering
{
    public class NaiveClusterAlgorithmTests
    {
        private readonly NaiveClusterAlgorithm _algorithm = new();

        private static IDistanceAlgorithm<string> CreateDistanceAlgorithm(IReadOnlyList<(string, string)> similarItems)
        {
            var distanceAlgorithm = new Mock<IDistanceAlgorithm<string>>();
            distanceAlgorithm.Setup(x => x.CalculateDistance(It.IsAny<string>(), It.IsAny<string>())).Returns(
                (string x, string y) =>
                {
                    if (similarItems.Contains((x, y)) || similarItems.Contains((y, x)))
                        return 0;

                    return 1;
                });

            return distanceAlgorithm.Object;
        }

        [Fact]
        public void BuildCluster_UnsimilarStrings_ReturnIndividualBuckets()
        {
            var input = new[] {"a", "b", "c"};

            var distance = CreateDistanceAlgorithm(Array.Empty<(string, string)>());
            var clusters = _algorithm.BuildCluster(input, s => s, distance, 0.5);

            AssertCluster(input.Select(x => new List<string> {x}), clusters);
        }

        [Fact]
        public void BuildCluster_AllSimilarStrings_ReturnSingleBucket()
        {
            var input = new[] {"a", "b", "c"};

            var distance = CreateDistanceAlgorithm(new[] {("a", "b"), ("a", "c"), ("b", "c")});
            var clusters = _algorithm.BuildCluster(input, s => s, distance, 0.5);

            AssertCluster(new[] {input}, clusters);
        }

        [Fact]
        public void BuildCluster_TransitiveSimilar_ReturnBuckets()
        {
            var input = new[] {"a", "b", "c"};

            var distance = CreateDistanceAlgorithm(new[] {("a", "b"), ("b", "c")});
            var clusters = _algorithm.BuildCluster(input, s => s, distance, 0.5);

            AssertCluster(new[] {input}, clusters);
        }

        [Fact]
        public void BuildCluster_SomeSimilar_ReturnBuckets()
        {
            var input = new[] {"a", "b", "c"};

            var distance = CreateDistanceAlgorithm(new[] {("a", "b")});
            var clusters = _algorithm.BuildCluster(input, s => s, distance, 0.5);

            AssertCluster(new[] {new[] {"a", "b"}, new[] {"c"}}, clusters);
        }

        [Fact]
        public void BuildCluster_IntegrationTest()
        {
            var input = new[]
                {"Visual Basic", "Visual Bäsic", "Wisual Basic", "VisualBasic", "C#", "C #", "C++", "Go", "Goo"};

            var distance = new NormalizedLevenshteinDistance();
            var clusters = _algorithm.BuildCluster(input, s => s, distance, NormalizedLevenshteinDistance.Threshold);

            AssertCluster(new[]
            {
                new[] {"Visual Basic", "Visual Bäsic", "Wisual Basic", "VisualBasic"}, new[] {"C#", "C #"},
                new[] {"C++"}, new[] {"Go", "Goo"},
            }, clusters);
        }

        private static void AssertCluster(IEnumerable<IReadOnlyList<string>> expected,
            IEnumerable<ClusterBucket<string>> cluster)
        {
            AssertHelper.AssertScrambledEquals(expected.Select(x => x.ToHashSet()),
                cluster.Select(x => x.Entities.ToHashSet()), HashSet<string>.CreateSetComparer());
        }
    }
}
