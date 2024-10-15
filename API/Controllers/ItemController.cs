using API.DTOs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController(ItemService item, ILogger<ItemController> log, AuthService auth)
        : ControllerBase
    {
        private readonly ItemService _item = item;
        private readonly ILogger<ItemController> _log = log;
        private readonly AuthService _auth = auth;

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
        )
        {
            var (items, count) = await _item.GetAllItems(page, limit);
            return Ok(
                new
                {
                    items,
                    count,
                    page,
                }
            );
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            try
            {
                var item = await _item.GetItem(id);
                return item != null
                    ? Ok(new { item = item })
                    : NotFound(new { message = "Item not found" });
            }
            catch (Exception ex)
            {
                _log.LogError("Error:{}", ex);
                return BadRequest($"Bad request:{ex}");
            }
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateItems(CreateItemDto[] items)
        {
            // if (_auth.GetUserRole()
            //     return Unauthorized(new { message = "You need to be an admin." });
            var (createdItems, failedItems) = await _item.CreateItem(items);
            if (failedItems != null)
            {
                Ok(
                    new
                    {
                        message = "Failed to create some items:",
                        failed = failedItems,
                        created = createdItems,
                    }
                );
            }
            if (createdItems.Count > 0)
                return BadRequest(
                    new { message = "Failed to create items: ", failed = failedItems }
                );
            return Ok(new { message = "Created items: ", created = createdItems });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateItems(UpdateItemDto[] items)
        {
            try
            {
                var (updatedItems, failedItems) = await _item.UpdateItems(items);

                if (failedItems.Count > 0)
                {
                    return Ok(
                        new
                        {
                            message = "Failed to update some items:",
                            failed = failedItems,
                            updated = updatedItems,
                        }
                    );
                }
                if (updatedItems.Count == 0)
                    return BadRequest(
                        new { message = "Failed to update items: ", failed = failedItems }
                    );
                return Ok(new { message = "Updated all items: ", items = updatedItems });
            }
            catch (Exception ex)
            {
                _log.LogError("Failed operation: {}", ex);
                return BadRequest(new { message = "Failed to perform operation" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItems(Guid[] items)
        {
            try
            {
                var (deletedItems, failedItems) = await _item.DeleteItems(items);
                if (failedItems.Count > 0)
                {
                    Ok(
                        new
                        {
                            message = "Failed to delete some items.",
                            failed = failedItems,
                            deleted = deletedItems,
                        }
                    );
                }
                if (deletedItems.Count == 0)
                {
                    return BadRequest(
                        new { message = "Failed to delete items. ", failed = failedItems }
                    );
                }
                return Ok(new { message = "Deleted all items.", deleted = deletedItems });
            }
            catch (Exception ex)
            {
                _log.LogError("Failed operation: {}", ex);
                return BadRequest(new { message = "Failed to perform operation." });
            }
        }
    }
}
