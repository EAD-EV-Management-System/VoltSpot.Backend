using Application.DTOs.Request.Users;
using Application.DTOs.Response.Auth;
using Application.DTOs.Response.Users;
using Application.UseCases.Bookings.Commands;
using Application.UseCases.Users.Commands;
using AutoMapper;
using Domain.Entities;
using VoltSpot.Application.DTOs;

namespace Application.Mappings
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Booking created successfully"))
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QrCode, opt => opt.MapFrom(src => src.QrCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateBookingRequestDto, CreateBookingCommand>();
            CreateMap<CancelBookingRequestDto, CancelBookingCommand>();
            CreateMap<UpdateBookingRequestDto, UpdateBookingCommand>();
        }
    }
}
