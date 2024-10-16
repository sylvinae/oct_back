using System.Collections;
using System.Text.RegularExpressions;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class ItemController(
        ItemService item,
        RestockService restock,
        ILogger<ItemController> log,
        AuthService auth
    ) : ControllerBase
    {
        private readonly RestockService _restock = restock;
        private readonly ItemService _item = item;
        private readonly ILogger<ItemController> _log = log;
        private readonly AuthService _auth = auth;

        [HttpPost("search")]
        public async Task<IActionResult> SearchItem(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
        )
        {
            _log.LogInformation("SearchItem called with query: {Query}", q);
            try
            {
                q = MyRegex().Replace(q, "").ToLower();
                var (result, count) = await _item.SearchItems(q, page, limit);
                _log.LogInformation("SearchItem completed. Items found: {Count}", count);

                return result.Length > 0
                    ? Ok(
                        new
                        {
                            message = "Items found",
                            items = result,
                            count,
                            page,
                        }
                    )
                    : NotFound(new { message = $"No items found for query: {q}" });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "SearchItem error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllItems(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10
        )
        {
            _log.LogInformation("GetAllItems called.");
            try
            {
                var (items, count) = await _item.GetAllItems(page, limit);
                _log.LogInformation("GetAllItems completed. Items count: {Count}", count);

                return Ok(
                    new
                    {
                        items,
                        count,
                        page,
                    }
                );
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "GetAllItems error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetItem(Guid id)
        {
            _log.LogInformation("GetItem called for ID: {Id}", id);
            try
            {
                var item = await _item.GetItem(id);
                if (item != null)
                {
                    _log.LogInformation("Item found for ID: {Id}", id);
                    return Ok(new { item });
                }
                _log.LogWarning("Item not found for ID: {Id}", id);
                return NotFound(new { message = "Item not found" });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "GetItem error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateItems([FromBody] List<ItemDto> items)
        {
            if (!(items is not IList))
            {
                return BadRequest(new { message = "Error: Wrong payload." });
            }
            _log.LogInformation("CreateItems called with {ItemCount} items.", items.Count);
            try
            {
                var (createdItems, failedItems) = await _item.CreateItem(items);
                _log.LogInformation(
                    "CreateItems completed. Successful: {SuccessCount}, Failed: {FailCount}",
                    createdItems.Count,
                    failedItems.Count
                );

                return failedItems.Count > 0
                    ? Ok(
                        new
                        {
                            message = "Some items failed to create.",
                            successful = createdItems,
                            failed = failedItems,
                        }
                    )
                    : Ok(
                        new
                        {
                            message = "All items created successfully.",
                            successful = createdItems,
                        }
                    );
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "CreateItems error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateItems([FromBody] List<UpdateItemDto> items)
        {
            _log.LogInformation("UpdateItems called with {ItemCount} items.", items.Count);
            try
            {
                var (updatedItems, failedItems) = await _item.UpdateItems(items);
                _log.LogInformation(
                    "UpdateItems completed. Updated: {UpdatedCount}, Failed: {FailCount}",
                    updatedItems.Count,
                    failedItems.Count
                );

                return failedItems.Count > 0
                    ? BadRequest(
                        new
                        {
                            message = "Some items failed to update.",
                            failed = failedItems,
                            successful = updatedItems,
                        }
                    )
                    : Ok(
                        new
                        {
                            message = "All items updated successfully.",
                            successful = updatedItems,
                        }
                    );
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "UpdateItems error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItems([FromBody] List<Guid> itemIds)
        {
            if (!ModelState.IsValid)
            {
                var sampleGuids = new[]
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                };
                return BadRequest(
                    new { message = "Expecting an array of ID's like so: ", items = sampleGuids }
                );
            }
            _log.LogInformation("DeleteItems called for {ItemCount} items.", itemIds.Count);
            try
            {
                var (deletedItems, failedItems) = await _item.DeleteItems(itemIds);
                _log.LogInformation(
                    "DeleteItems completed. Deleted: {DeletedCount}, Failed: {FailCount}",
                    deletedItems.Count,
                    failedItems.Count
                );

                return failedItems.Count > 0
                    ? BadRequest(
                        new
                        {
                            message = "Some items failed to delete.",
                            failed = failedItems,
                            successful = deletedItems,
                        }
                    )
                    : Ok(
                        new
                        {
                            message = "All items deleted successfully.",
                            successful = deletedItems,
                        }
                    );
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "DeleteItems error occurred.");
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("restock")]
        public async Task<IActionResult> RestockItems([FromBody] List<RestockDto> restockItems)
        {
            if (restockItems is not IList)
            {
                return BadRequest(new { message = "Expected an array of items." });
            }
            _log.LogInformation("RestockItems called for {ItemCount} items.", restockItems.Count);
            try
            {
                var (restocked, added) = await _restock.Restock(restockItems);
                _log.LogInformation(
                    "RestockItems completed. Restocked: {RestockedCount}, Added: {AddedCount}",
                    restocked.Count,
                    added.Count
                );
                if (restocked.Count == 0 && added.Count == 0)
                    return BadRequest(new { message = "No items restocked or added." });

                var response = new
                {
                    message = "Operation completed.",
                    restocked = restocked.Count > 0 ? restocked : null,
                    added = added.Count > 0 ? added : null,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _log.LogError("RestockItems error: {Ex}", ex);
                return BadRequest(new { message = $"Error: {ex.Message}" });
            }
        }

        [GeneratedRegex("[^a-zA-Z0-9 ]")]
        private static partial Regex MyRegex();
    }
}
