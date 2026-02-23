using Microsoft.AspNetCore.Mvc; 

namespace TrainEase.APIs.Controllers.Controllers.Base
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")] 
    public class BaseApiController : ControllerBase 
    {
    }
}