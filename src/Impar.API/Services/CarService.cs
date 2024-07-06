using AutoMapper;
using Impar.Infra.Context;
using ImparTesteAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Impar.Domain.Models;
using ImparTesteAPI.DTOs.Car;
using ImparTesteAPI.Utilities;

namespace ImparTesteAPI.Services
{
	public class CarService : ICarService
	{
		private readonly EntityContext _context;
		private readonly IMapper _mapper;

		public CarService(EntityContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<List<CarReadDto>> GetAllCarsAsync(PaginationAndSearchParameters paginationAndSearchParameters)
		{
			var query = _context.Cars.AsQueryable();
			if (!string.IsNullOrEmpty(paginationAndSearchParameters.SearchName))
			{
				query = query.Where(car => car.Name.Contains(paginationAndSearchParameters.SearchName));
			}

			var car = await query
				.Skip((paginationAndSearchParameters.PageNumber - 1) * paginationAndSearchParameters.PageSize)
				.Take(paginationAndSearchParameters.PageSize)
				.Include(c => c.Photo)
				.ToListAsync();

			var carsDto = _mapper.Map<List<CarReadDto>>(car);

			return carsDto;
		}

		public async Task<CarReadDto> GetCarByIdAsync(int id)
		{
			var car = await _context.Cars.Include(c => c.Photo).FirstOrDefaultAsync(c => c.Id == id);

			if (car == null)
			{
				return null;
			}

			var carDto = _mapper.Map<CarReadDto>(car);

			return carDto;
		}

		public async Task<CarReadDto> CreateCarAsync(CarCreateDto carCreateDto)
		{
			var car = _mapper.Map<Car>(carCreateDto);

			_context.Cars.Add(car);
			await _context.SaveChangesAsync();

			if (carCreateDto.ImageFile.Length > 0)
			{
				using var ms = new MemoryStream();
				await carCreateDto.ImageFile.CopyToAsync(ms);
				var fileBytes = ms.ToArray();
				string base64String = Convert.ToBase64String(fileBytes);

				var photo = new Photo
				{
					Base64 = base64String,
					CarId = car.Id
				};

				_context.Photos.Add(photo);
				await _context.SaveChangesAsync();

				car.Photo = photo;
			}

			var carDto = _mapper.Map<CarReadDto>(car);

			return carDto;
		}

		public async Task DeleteCarAsync(int id)
		{
			var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);

			if (car == null)
			{
				return;
			}

			_context.Cars.Remove(car);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateCarAsync(int id, CarUpdateDto carUpdateDto)
		{
			var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);

			if (car == null)
			{
				return;
			}

			car.Name = carUpdateDto.Name;
			if (carUpdateDto.ImageFile != null && carUpdateDto.ImageFile.Length > 0)
			{
				using var ms = new MemoryStream();
				await carUpdateDto.ImageFile.CopyToAsync(ms);
				var fileBytes = ms.ToArray();
				var base64String = Convert.ToBase64String(fileBytes);

				var photo = await _context.Photos.FirstOrDefaultAsync(p => p.CarId == car.Id);

				if (photo == null)
				{
					photo = new Photo
					{
						Base64 = base64String,
						CarId = car.Id
					};

					_context.Photos.Add(photo);
				}
				else
				{
					photo.Base64 = base64String;
				}
			}

			await _context.SaveChangesAsync();
		}
	}
}
