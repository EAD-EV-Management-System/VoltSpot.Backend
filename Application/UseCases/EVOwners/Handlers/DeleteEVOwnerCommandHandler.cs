using Application.UseCases.EVOwners.Commands;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.EVOwners.Handlers
{
    public class DeleteEVOwnerCommandHandler : IRequestHandler<DeleteEVOwnerCommand, Unit>
    {
        private readonly IEVOwnerRepository _evOwnerRepository;

        public DeleteEVOwnerCommandHandler(IEVOwnerRepository evOwnerRepository)
        {
            _evOwnerRepository = evOwnerRepository;
        }

        public async Task<Unit> Handle(DeleteEVOwnerCommand request, CancellationToken cancellationToken)
        {
            var evOwner = await _evOwnerRepository.GetByNICAsync(request.NIC);
            
            if (evOwner == null)
            {
                throw new KeyNotFoundException($"EV Owner with NIC {request.NIC} not found");
            }

            // Soft delete by setting IsDeleted flag
            evOwner.IsDeleted = true;
            evOwner.UpdatedAt = DateTime.UtcNow;

            await _evOwnerRepository.UpdateAsync(evOwner);
            
            return Unit.Value;
        }
    }
}