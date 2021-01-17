namespace PaderConference.Core.Interfaces.Gateways.Repositories
{
    public enum OptimisticUpdateResult
    {
        Ok,
        ConcurrencyException,
        DeletedException,
    }
}
