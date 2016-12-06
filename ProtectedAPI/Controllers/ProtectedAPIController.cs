using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ProtectedAPI.Controllers
{
    [Route("api")]
    public class ProtectedAPIController : Controller
    {
        [Route("protected")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Protected()
        {
            return await Task.FromResult(new JsonResult(new { Success = true, Result = "Protected API call result." }));
        }
    }
}
