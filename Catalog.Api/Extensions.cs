using Catalog.Api.Entities;
using Catalog.Api.DTOs;

namespace Catalog.Api
{
    public static class Extensions
    {
        public static ItemDTO AsDTO(this Item item)
        {
            return new ItemDTO(item.Id, item.Name!, item.Price, item.CreatedDate);
        }
    }
}
