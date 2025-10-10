using Application.DTOs.Request.EVOwners;
using Application.DTOs.Response.EVOwners;
using Application.UseCases.EVOwners.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class EVOwnerMappingProfile : Profile
    {
        public EVOwnerMappingProfile()
        {
            CreateMap<EVOwner, EVOwnerResponseDto>();

            CreateMap<RegisterEVOwnerRequestDto, RegisterEVOwnerCommand>();

            CreateMap<EVOwnerLoginRequestDto, LoginEVOwnerCommand>();
            
            CreateMap<UpdateEVOwnerRequestDto, UpdateEVOwnerCommand>();
        }
    }
}
