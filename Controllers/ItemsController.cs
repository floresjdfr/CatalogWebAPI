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
        public IEnumerable<ItemDTO> GetItems()
        {
            var items = _repository.GetItems().Select(item => item.AsDTO());
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDTO> GetItemById(Guid id)
        {
            var item = _repository.GetItemById(id);
            return item is null ? NotFound() : item.AsDTO();
        }


        [HttpPost]
        public ActionResult<ItemDTO> CreateItem(CreateItemDTO item)
        {
            Item newItem = new()
            {
                Id = Guid.NewGuid(),
                Name = item.Name,
                Price = item.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            _repository.CreateItem(newItem);

            //
            return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, newItem.AsDTO());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDTO itemDTO)
        {
            var existingItem = _repository.GetItemById(id);
            if (existingItem is null)
                return NotFound();
            
            Item item = existingItem with
            {
                Name = itemDTO.Name,
                Price = itemDTO.Price
            };

            _repository.UpdateItem(item);
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = _repository.GetItemById(id);
            if (existingItem is null)
                return NotFound();

            _repository.DeleteItem(id);

            return NoContent();
        }
    }
}
