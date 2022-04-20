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
    public class LoginController : ControllerBase
    {       
        private static IConfiguration _config;
        public LoginController(IHttpContextAccessor accessor, IConfiguration config)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]dynamic obj)
        {
            try
            {
                var RememberMe = obj.RememberMe == null ? false : (bool)obj.RememberMe;

                SP_Login_User sp = new SP_Login_User();
                sp.UserName = (string)obj.username;
                sp.Password = AESEncrytDecry.EncryptStringAES((string)obj.password);

                var res = DataManager.ExecuteSPGetSingle<User_Account_Model, SP_Login_User>(sp);
                if (res != null && res.UserID > 0)
                {
                    // authentication successful so generate jwt token
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>{
                          //new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
                          new Claim(ClaimTypes.NameIdentifier, res.UserID.ToString()),
                          new Claim(Claim_Types.UserName, res.UserName.ToString()),
                          new Claim(Claim_Types.DisplayName, res.DisplayName),
                          new Claim(Claim_Types.Email, res.Email),
                          new Claim(Claim_Types.Is_Agent, res.Is_Agent.ToString()),
                          new Claim(Claim_Types.Is_Client, res.Is_Client.ToString())
                        };

                    var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                        _config["Jwt:Issuer"],
                        claims,
                        expires: DateTime.UtcNow.AddYears(1),
                        signingCredentials: credentials);

                    string strToken = new JwtSecurityTokenHandler().WriteToken(token);

                    //moved in admin controller in Get_Account_Detail api
                    //HtmlHelpers.SetResource_Cache();//Set Global Resources for invoice settings 

                    return Ok(ComUti.Get_ApiResponse(strToken));
                }
                else
                {
                    return Ok(ComUti.Get_ApiResponse("", "Invalid UserName OR Password.", false));
                }
            }
            catch (Exception ex)
            {
                return Ok(ComUti.Get_ApiResponse("", "Error Occures.", false));
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Ok();
        }

    }
}