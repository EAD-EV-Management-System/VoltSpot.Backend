
namespace Domain.Enums
{
    public enum BookingStatus
    {
        Pending,      // Waiting for station operator approval
        Confirmed,    // Approved and ready for charging
        Cancelled,    // Cancelled by user or operator
        Completed,    // Charging session completed
        NoShow        // User didn't show up
    }
}

// Domain/Enums/ChargerType.cs
namespace Domain.Enums
{
    public enum ChargerType
    {
        AC,   // AC charging
        DC    // DC fast charging
    }
}