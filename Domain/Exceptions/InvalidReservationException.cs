
namespace Domain.Exceptions
{
    public class InvalidReservationException : Exception
    {
        public InvalidReservationException(string message) : base(message) { }
        public InvalidReservationException(string message, Exception innerException) : base(message, innerException) { }
    }
}