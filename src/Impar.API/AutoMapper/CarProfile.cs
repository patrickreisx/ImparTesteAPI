using AutoMapper;
using Impar.Domain.Models;
using ImparTesteAPI.DTOs.Car;

namespace ImparTesteAPI.AutoMapper;

public class CarProfile : Profile
{
	public CarProfile()
	{
		CreateMap<Car, CarReadDto>()
			.ForMember(
				dest => dest.Base64,
				opt => opt.MapFrom(
					src => src.Photo.Base64
				));
		CreateMap<CarReadDto, Car>();

		CreateMap<Car, CarCreateDto>();
		CreateMap<CarCreateDto, Car>();
	}
}
