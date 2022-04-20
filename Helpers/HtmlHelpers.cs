using MalVirDetector_CLI_API.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Web.Helpers
{
    public class HtmlHelpers
    {
        public static IHttpContextAccessor _httpContextAccessor;
        public HtmlHelpers(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static long GetUserId()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User;
            var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
                return 0;
            else
                return Convert.ToInt64(claim.Value);
        }

        public static string GlobalResources(string key, string lang = "")
        {
            var resources = CacheManager.GetFromGlobal<dynamic>(lang);
            if (resources != null)
            {
                if (resources[key] != null)
                {
                    return resources[key].ToString();
                }
                else
                {
                    return key + "_XXXXX";
                }
            }
            else
            {
                SetResource_Cache(lang);
                resources = CacheManager.GetFromGlobal<dynamic>(lang);
                if (resources[key] != null) { return resources[key].ToString(); }
                return key + "_XXXXX";
            }
        }
        public static void SetResource_Cache(string lang)
        {
            var cache = CacheManager.GetFromGlobal<dynamic>(lang);
            if (cache == null)
            {
                var jsondata = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents/Resources", lang + ".json")));
                var res = JsonConvert.DeserializeObject<dynamic>(jsondata);

                CacheManager.AddToGlobal<dynamic>(lang, res, DateTime.Now.AddHours(1));
            }
        }

        public class ClaimsModelService
        {
            private static IHttpContextAccessor _accessor;
            private static IConfiguration _config;
            public ClaimsModelService(IHttpContextAccessor accessor, IConfiguration config)
            {
                _accessor = accessor;
                _config = config;
            }
            public static long GetUserId()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return 0;
                else
                    return Convert.ToInt64(claim.Value);
            }
            public static string GetUserName()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.UserName);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return "";
                else
                    return Convert.ToString(claim.Value);
            }
            public static string GetDisplayName()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.DisplayName);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return "";
                else
                    return Convert.ToString(claim.Value);
            }
            public static string GetEmail()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Email);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return "";
                else
                    return Convert.ToString(claim.Value);
            }
            public static string Gettype()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.type);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return "";
                else
                    return Convert.ToString(claim.Value);

            }
            public static bool GetIs_Admin()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Is_Admin);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return false;
                else
                    return Convert.ToBoolean(claim.Value);
            }
            public static bool GetIs_Agent()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Is_Agent);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return false;
                else
                    return Convert.ToBoolean(claim.Value);
            }
            public static bool GetIs_Client()
            {
                var claimsIdentity = _accessor.HttpContext.User;
                var claim = claimsIdentity.Claims.FirstOrDefault(d => d.Type == Claim_Types.Is_Client);
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                    return false;
                else
                    return Convert.ToBoolean(claim.Value);
            }

            public static SMPTSetting getSMTP()
            {
                SMPTSetting setting = new SMPTSetting()
                {
                    fromEmailAlias = _config["smtp:fromEmailAlias"],
                    fromEmail = _config["smtp:fromEmail"],
                    Username = _config["smtp:Username"],
                    Password = _config["smtp:Password"],
                    SmtpServer = _config["smtp:SmtpServer"],
                    SmtpPort = Convert.ToInt32(_config["smtp:SmtpPort"]),
                    EnableSsl = Convert.ToBoolean(_config["smtp:EnableSsl"]),
                    DefaultCredentials = Convert.ToBoolean(_config["smtp:DefaultCredentials"])
                };
                return setting;
            }
            public static void UpdateSMTP(string key, string value)
            {
                //_config["smtp:" + key + ""] = value;
            }

            public static string GetAppSetting(string key)
            {
                return _config["AppSettings:" + key + ""];
            }

            public static string GetConnectionString()
            {
                return _config.GetConnectionString("conn");
            }
            public static string GetAPI_Url()
            {
                return _accessor.HttpContext.Request.Scheme + "://" + _accessor.HttpContext.Request.Host.Value + "/";
            }
        }
    }
}
