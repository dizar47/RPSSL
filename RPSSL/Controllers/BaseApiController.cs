using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RPSSL.Controllers
{
    public class BaseApiController : Controller
    {
        protected ILogger logger;
        public BaseApiController(ILogger logger)
        {

            this.logger = logger;
        }
    }
}
