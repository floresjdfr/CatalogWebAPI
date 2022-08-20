using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.DTOs
{
    public record ItemDTO(Guid Id, string Name, decimal Price, DateTimeOffset CreatedDate);
    public record  CreateItemDTO([Required] string Name,[Required] [Range(1, int.MaxValue)] decimal Price);
    public record  UpdateItemDTO([Required] string Name,[Required] [Range(1, int.MaxValue)] decimal Price);
    
}