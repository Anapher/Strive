namespace Strive.Core.Services.Poll.Types.TagCloud.Clustering
{
    public interface IDistanceAlgorithm<in TValue>
    {
        double CalculateDistance(TValue x, TValue y);
    }
}
