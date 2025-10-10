using MediatR;

namespace Application.UseCases.EVOwners.Commands
{
    public class DeleteEVOwnerCommand : IRequest<Unit>
    {
        public string NIC { get; set; } = string.Empty;
    }
}