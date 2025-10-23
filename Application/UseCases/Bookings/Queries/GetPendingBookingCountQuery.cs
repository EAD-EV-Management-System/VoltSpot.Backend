using MediatR;


namespace Application.UseCases.Bookings.Queries
{
    public class GetPendingBookingCountQuery : IRequest<int>
    {
        public string EvOwnerNic { get; set; } = string.Empty;
    }
}
