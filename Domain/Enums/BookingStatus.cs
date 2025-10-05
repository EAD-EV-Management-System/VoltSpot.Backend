namespace Domain.Enums
{
    public enum BookingStatus
    {
        Pending,      // Waiting for station operator approval
        Confirmed,    // Approved and ready for charging
        Cancelled,    // Cancelled by user or operator
        Completed,    // Charging session completed
        NoShow
    }
}
