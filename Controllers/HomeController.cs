using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MalVirDetector_CLI_API.Web.Helpers;
using MalVirDetector_CLI_API.Logic;
using MalVirDetector_CLI_API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Configuration;

//using System.DirectoryServices;
using Newtonsoft.Json;

namespace MalVirDetector_CLI_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private static IConfiguration _config;
        protected ICompositeViewEngine viewEngine;
        public HomeController(ICompositeViewEngine viewEngine, IHttpContextAccessor accessor, IConfiguration config)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);

            this.viewEngine = viewEngine;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("");
        }

        public IActionResult Get_Translate(string lang = "en")
        {
            var jsondata = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents/Resources", lang + ".json")));
            var res = JsonConvert.DeserializeObject<dynamic>(jsondata);
            return Ok(res);
        }
        [HttpPost]
        public List<KeyValueString> Get_Languages()
        {
            var res = User_Manager.Get_Languages();
            return res;
        }
        [HttpPost]
        public IActionResult Get_DefaultLang()
        {
            var res = General_Setting_Manager.Get_DefaultLang();
            return Ok(ComUti.Get_ApiResponse(res));
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            viewName = viewName ?? ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IView view = viewEngine.FindView(ControllerContext, viewName, true).View;
                ViewContext viewContext = new ViewContext(ControllerContext, view, ViewData, TempData, sw, new HtmlHelperOptions());

                view.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public int ForgotPassword(dynamic obj)
        {
            var res = User_Manager.ForgotPassword((string)obj.UserName, (string)obj.site_url);
            return res;
        }
        [HttpPost]
        public IActionResult Decrypt_ForgotPassword(dynamic obj)
        {
            var res = User_Manager.Decrypt_ForgotPassword((string)obj.Key);
            return Ok(ComUti.Get_ApiResponse(res));
        }
        [HttpPost]
        public int ResetPassword(dynamic obj)
        {
            var res = User_Manager.ResetPassword((string)obj.UserName, (string)obj.Email, (string)obj.Password);
            return res;
        }

    }
}