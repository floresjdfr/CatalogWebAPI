using Microsoft.AspNetCore.Mvc;
using  Catalog.Api.Repositories;
using  Catalog.Api.Entities;
using  Catalog.Api.DTOs;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(IItemsRepository repository, ILogger<ItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await _repository.GetItemsAsync())
                .Select(item => item.AsDto());
            
            _logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {items.Count()}");
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemByIdAsync(Guid id)
        {
            var item = await _repository.GetItemByIdAsync(id);
            return item is null ? NotFound() : item.AsDto();
        }


        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto item)
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
            var actionName = nameof(GetItemByIdAsync);
            return CreatedAtAction( actionName, new { id = newItem.Id }, newItem.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await _repository.GetItemByIdAsync(id);
            if (existingItem is null)
                return NotFound();
            
            existingItem.Name = itemDto.Name;
            existingItem.Price = itemDto.Price;


            await _repository.UpdateItemAsync(existingItem);
            
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
