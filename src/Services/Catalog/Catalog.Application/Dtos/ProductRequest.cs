using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.Dtos;

public record ProductRequest(
    [Required, StringLength(200)] string Name,
    [StringLength(2000)] string Description,
    [Required, StringLength(100)] string Category,
    [Range(0.01, 1_000_000)] decimal Price,
    [Range(0, int.MaxValue)] int Stock);
