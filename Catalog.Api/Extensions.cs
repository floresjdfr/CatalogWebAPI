using Catalog.Api.Entities;
using Catalog.Api.DTOs;

namespace Catalog.Api
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name!, item.Price, item.CreatedDate);
        }
    }
}
