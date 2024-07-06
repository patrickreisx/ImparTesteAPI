namespace Impar.Domain.Models;

public class Photo
{
	public int Id { get; set; }
	public string Base64 { get; set; } = null!;

	public int CarId { get; init; }

	public Car Car { get; set; } = null!;
}
