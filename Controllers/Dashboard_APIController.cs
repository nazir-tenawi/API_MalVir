using MalVirDetector_CLI_API.Logic;
using MalVirDetector_CLI_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MalVirDetector_CLI_API.Web.Helpers;

namespace MalVirDetector_CLI_API.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private static IConfiguration _config;
        public DashboardController(IHttpContextAccessor accessor, IConfiguration config)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);
        }



    }
}