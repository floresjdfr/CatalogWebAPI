using Microsoft.AspNetCore.Mvc;
using Catalog.Repositories;
using Catalog.Entities;
using Catalog.DTOs;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;

        public ItemsController(IItemsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDTO>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync())
                .Select(item => item.AsDTO());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemByIdAsync(Guid id)
        {
            var item = await _repository.GetItemByIdAsync(id);
            return item is null ? NotFound() : item.AsDTO();
        }


        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItemAsync(CreateItemDTO item)
        {
            Item newItem = new()
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Price = item.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await _repository.CreateItemAsync(newItem);

            //
            return CreatedAtAction(nameof(GetItemByIdAsync), new { id = newItem.Id }, newItem.AsDTO());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDTO itemDTO)
        {
            var existingItem = await _repository.GetItemByIdAsync(id);
            if (existingItem is null)
                return NotFound();
            
            Item item = existingItem with
            {
                Name = itemDTO.Name,
                Price = itemDTO.Price
            };

            await _repository.UpdateItemAsync(item);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await _repository.GetItemByIdAsync(id);
            if (existingItem is null)
                return NotFound();

            await _repository.DeleteItemAsync(id);

            return NoContent();
        }
    }
}
