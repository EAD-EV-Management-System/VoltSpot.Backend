using Application.DTOs.Response.EVOwners;
using MediatR;

namespace Application.UseCases.EVOwners.Queries
{
    public class GetAllEVOwnersQuery : IRequest<List<EVOwnerResponseDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
    }
}