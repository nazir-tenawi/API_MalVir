using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MalVirDetector_CLI_API.Logic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MalVirDetector_CLI_API.Model;
using MalVirDetector_CLI_API.Web.Helpers;

namespace MalVirDetector_CLI_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ScanController : ControllerBase
    {       
        private static IConfiguration _config;
        public ScanController(IHttpContextAccessor accessor, IConfiguration config)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Scan_Search([FromBody]dynamic obj)
        {
            var hash = Scan_Manager.Scan_Search((string)obj.HashContent, ClaimsModel.UserId);
            var res2 = History_Manager.History_Create(hash, ClaimsModel.UserId, 1);

            if(hash != null){
                return Ok(hash);
            } else {
                return new NoContentResult();
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Scan_File(dynamic obj)
        {
            var hash = Scan_Manager.Scan_File(obj.attachment.ToObject<File_Attachments>(), ClaimsModel.UserId);
            var res2 = History_Manager.History_Create(hash, ClaimsModel.UserId, 2);
            if(hash != null){
                return Ok(hash);
            } else {
                return new NoContentResult();
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult Scan_Wireshark(dynamic obj)
        {
            var res = Scan_Manager.Scan_Wireshark(obj.attachment.ToObject<File_Attachments>(), ClaimsModel.UserId);
            if(res != null){
                return Ok(res);
            } else {
                return new NoContentResult();
            }        
        }
    
    }

    
}