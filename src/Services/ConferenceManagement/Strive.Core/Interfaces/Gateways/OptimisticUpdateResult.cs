namespace Strive.Core.Interfaces.Gateways
{
    public enum OptimisticUpdateResult
    {
        Ok,
        ConcurrencyException,
        DeletedException,
    }
}
