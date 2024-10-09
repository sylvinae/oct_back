using Microsoft.AspNetCore.Mvc;

namespace Api.controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult WrapResponse<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem("Error: " + ex);
            }
        }
    }
}
