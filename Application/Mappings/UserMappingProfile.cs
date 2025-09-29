using Application.DTOs.Request.Users;
using Application.DTOs.Response.Auth;
using Application.DTOs.Response.Users;
using Application.UseCases.Users.Commands;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserResponseDto>();

            CreateMap<User, UserInfoResponseDto>();

            CreateMap<CreateUserRequestDto, CreateUserCommand>();

            CreateMap<UpdateUserRequestDto, UpdateUserCommand>();
        }
    }
}
