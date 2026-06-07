namespace Orders.Api.Exceptions;

public sealed class StockReservationFailedException : Exception
{
    public StockReservationFailedException()
        : base("Unable to reserve stock for one or more order lines.")
    {
    }
}
