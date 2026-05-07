using Gold.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Gold_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.Succeeded)
        {
            return Ok(result.Data);
        }
        return BadRequest(new { error = result.Error, errors = result.Errors });
    }

    protected IActionResult ToActionResult(Result result)
    {
        if (result.Succeeded)
        {
            return NoContent();
        }
        return BadRequest(new { error = result.Error, errors = result.Errors });
    }
}
