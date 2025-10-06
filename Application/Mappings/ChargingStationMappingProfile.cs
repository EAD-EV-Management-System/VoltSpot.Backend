using Application.DTOs.Response.ChargingStations;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class ChargingStationMappingProfile : Profile
    {
        public ChargingStationMappingProfile()
        {
            CreateMap<ChargingStation, ChargingStationResponseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational()))
                .ForMember(dest => dest.IsOperatingNow, opt => opt.MapFrom(src => src.IsOperatingNow()))
                .ForMember(dest => dest.OperatingHours, opt => opt.MapFrom(src => src.OperatingHours));

            CreateMap<OperatingHours, OperatingHoursResponseDto>();
        }
    }
}