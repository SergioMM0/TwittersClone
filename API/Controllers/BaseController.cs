using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    
    /// <summary>
    /// Parent class for every Controller. Interface-like class
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase {
        
    }
}
