using System.ComponentModel.DataAnnotations;

namespace Catalog.DTOs
{
    public record CreateItemDTO
    {
        [Required]
        public string Name { get; init; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public decimal Price { get; init; }
    }
}
