using System.Collections;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestockController(
        RestockService restock,
        ILogger<RestockController> log,
        AuthService auth
    ) : ControllerBase
    {
        private readonly RestockService _restock = restock;
        private readonly ILogger<RestockController> _log = log;
        private readonly AuthService _auth = auth;

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
    }
}
