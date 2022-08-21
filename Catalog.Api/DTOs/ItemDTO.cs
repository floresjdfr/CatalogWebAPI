using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record ItemDto(Guid Id, string Name, decimal Price, DateTimeOffset CreatedDate);
    public record  CreateItemDto([Required] string Name,[Required] [Range(1, int.MaxValue)] decimal Price);
    public record  UpdateItemDto([Required] string Name,[Required] [Range(1, int.MaxValue)] decimal Price);
    
}