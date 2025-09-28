using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Base
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success")
        {
            return Ok(new
            {
                Success = true,
                Message = message,
                Data = data
            });
        }

        protected IActionResult Success(string message = "Success")
        {
            return Ok(new
            {
                Success = true,
                Message = message
            });
        }

        protected IActionResult Error(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, new
            {
                Success = false,
                Message = message
            });
        }
    }
}
