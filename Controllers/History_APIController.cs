using MalVirDetector_CLI_API.Logic;
using MalVirDetector_CLI_API.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MalVirDetector_CLI_API.Web.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace MalVirDetector_CLI_API.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private static IConfiguration _config;
        public HistoryController(IHttpContextAccessor accessor, IConfiguration config)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);
        }

        [HttpPost]
        public List<History_Model> Get_History_List(dynamic obj)
        {
            var res = History_Manager.Get_History_List((bool)obj.Is_Agent, (bool)obj.Is_Client, ClaimsModel.UserId);
            return res;
        }
        [HttpPost]
        public History_Model Get_History_ByID(dynamic obj)
        {
            var res = History_Manager.Get_History_ByID((long)obj.HistoryID);
            return res;
        }
        // [HttpPost]
        // public Tuple<long, string> History_Create(dynamic obj)
        // {
        //     var res = History_Manager.History_Create(obj.model.ToObject<History_Model>(), ClaimsModel.UserId);
        //     return res;
        // }
        // [HttpPost]
        // public Tuple<long, string> History_Update(dynamic obj)
        // {
        //     var res = History_Manager.History_Update(obj.model.ToObject<History_Model>(), ClaimsModel.UserId);
        //     return res;
        // }

        [HttpPost]
        public long History_Delete(dynamic obj)
        {
            var res = History_Manager.History_Delete((string)obj.HistoryIDs);
            return res;
        }

    }

}