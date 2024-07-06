using ImparTesteAPI.DTOs;
using ImparTesteAPI.DTOs.Car;
using ImparTesteAPI.Services.Interfaces;
using ImparTesteAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace ImparTesteAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{

	private readonly ICarService _carService;
	public CarController(ICarService carService)
	{
		_carService = carService;
	}	

	[HttpGet]
	public async Task<ActionResult> Index([FromQuery] PaginationAndSearchParameters paginationAndSearchParameters)
	{
		var carDtos = await _carService.GetAllCarsAsync(paginationAndSearchParameters);
		return Ok(carDtos);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult> Get(int id)
	{
		var carDto = await _carService.GetCarByIdAsync(id);
		return Ok(carDto);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromForm] CarCreateDto carCreateDto)
	{
		var carDto = await _carService.CreateCarAsync(carCreateDto);
		return Ok(carDto);
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult> Update(int id, [FromForm] CarUpdateDto carUpdateDto)
	{
		await _carService.UpdateCarAsync(id, carUpdateDto);
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<ActionResult> Delete(int id)
	{
		await _carService.DeleteCarAsync(id);
		return Ok();
	}
}
