using MalVirDetector_CLI_API.Logic;
using MalVirDetector_CLI_API.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using MalVirDetector_CLI_API.Web.Helpers;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace MalVirDetector_CLI_API.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private static IConfiguration _config;
        private static IWebHostEnvironment _env;
        public AdminController(IHttpContextAccessor accessor, IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            var helper = new HtmlHelpers.ClaimsModelService(accessor, config);
            _env = env;
        }


        [HttpPost]
        public Page_Permission_Model Get_Account_Detail()
        {
            var res = User_Manager.Page_Permission(ClaimsModel.UserId);
            return res;
        }




        #region Admin Basic
        #region Hash
        [HttpPost]
        public List<Hash_Model> Get_Hash_List()
        {
            var res = Admin_Basic_Manager.Get_Hash_List();
            return res;
        }
        [HttpPost]
        public Hash_Model Get_Hash_ByID(dynamic obj)
        {
            var res = Admin_Basic_Manager.Get_Hash_ByID((long)obj.HashID);
            return res;
        }
        [HttpPost]
        public long Hash_Update(Hash_Model model)
        {
            var res = Admin_Basic_Manager.Hash_Update(model);
            return res;
        }
        [HttpPost]
        public long Hash_Delete(dynamic obj)
        {
            var res = Admin_Basic_Manager.Hash_Delete((string)obj.HashIDs);
            return res;
        }
        #endregion
        #endregion

        #region Admin Common Setting

        #region AgentSetting
        [HttpPost]
        public AgentSetting_Model Get_AgentSetting()
        {
            var res = General_Setting_Manager.Get_AgentSetting();
            return res;
        }
        [HttpPost]
        public long AgentSetting_Update(AgentSetting_Model model)
        {
            var res = General_Setting_Manager.AgentSetting_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        #endregion

        #region ClientSetting
        [HttpPost]
        public ClientSetting_Model Get_ClientSetting()
        {
            var res = General_Setting_Manager.Get_ClientSetting();
            return res;
        }
        [HttpPost]
        public long ClientSetting_Update(ClientSetting_Model model)
        {
            var res = General_Setting_Manager.ClientSetting_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        #endregion

        #region ApplicationSetting               
        [HttpPost]
        public ApplicationSetting_Model Get_ApplicationSetting()
        {
            var res = General_Setting_Manager.Get_ApplicationSetting();
            return res;
        }
        [HttpPost]
        public long ApplicationSetting_Update(ApplicationSetting_Model model)
        {
            var res = General_Setting_Manager.ApplicationSetting_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [HttpPost]
        public long Logo_Update(ApplicationSetting_Model model)
        {
            var res = General_Setting_Manager.Logo_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        #endregion

        #region Mail Settings
        [HttpPost]
        public Outgoing_Server_Model Get_Outgoing_Server()
        {
            var res = Mail_Setting_Manager.Get_Outgoing_Server();
            return res;
        }
        [HttpPost]
        public int Update_Outgoing_Server(Outgoing_Server_Model model)
        {
            var file_name = "appsettings.json";
            if (_env.EnvironmentName.ToLower() != "development")
            {
                file_name = "appsettings." + _env.EnvironmentName + ".json";
            }
            var res = Mail_Setting_Manager.Update_Outgoing_Server(model, file_name);
            return res;
        }
        [HttpPost]
        public int Test_Outgoing_Server(Outgoing_Server_Model model)
        {
            var res = Mail_Setting_Manager.Test_Outgoing_Server(model);
            return res;
        }
        #endregion

        #endregion

        #region UserManagement

        #region User

        [HttpPost]
        public int ChangePassword(dynamic obj)
        {
            var res = User_Manager.ChangePassword(ClaimsModel.UserId, (string)obj.Password);
            return res;
        }
        [HttpPost]
        public int Check_UserName_Available(dynamic obj)
        {
            var res = User_Manager.Check_UserName_Available((long)obj.UserID, (string)obj.UserName);
            return res;
        }

        [HttpPost]
        public List<User_Account_Model> Get_UserManagement_List(dynamic obj)
        {
            var res = User_Manager.Get_UserManagement_List((bool)obj.Is_Agent, (bool)obj.Is_Client);
            return res;
        }
        //[HttpPost]
        //public List<User_Account_Model> Get_Agent_Client_List(dynamic obj)
        //{
        //    var res = User_Manager.Get_UserManagement_List((bool)obj.Is_Agent, (bool)obj.Is_Active);
        //    return res;
        //}

        [HttpPost]
        public List<User_Account_Model> Get_UserSelection_List(dynamic obj)
        {
            var res = User_Manager.Get_UserSelection_List((bool)obj.Is_Agent);
            return res;
        }
        [HttpPost]
        public User_Account_Model Get_UserManagement_ByID(dynamic obj)
        {
            var res = User_Manager.Get_UserManagement_ByID((long)obj.UserID);
            return res;
        }
        [HttpPost]
        public long UserManagement_Update(User_Account_Model model)
        {
            var res = User_Manager.UserManagement_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [AllowAnonymous]
        [HttpPost]
        public long UserManagement_Signup(User_Account_Model model)
        {
			model.RoleID = 3;
            var res = User_Manager.UserManagement_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [HttpPost]
        public int UserManagement_Delete(dynamic obj)
        {
            var res = User_Manager.UserManagement_Delete((string)obj.UserIDs);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [HttpPost]
        public int ActiveDeActive_User(List<KeyValue> lstUsers)
        {
            var res = User_Manager.ActiveDeActive_User(lstUsers);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [HttpPost]
        public int ResetDefaultPassword_User(dynamic obj)
        {
            var res = User_Manager.ResetDefaultPassword_User((string)obj.UserIDs, (string)obj.DefaultPassword);
            return res;
        }

        #region Requester Profile
        [HttpPost]
        public long Requester_UserManagement_Update(User_Account_Model model)
        {
            var res = User_Manager.Requester_UserManagement_Update(model);
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        #endregion

        #endregion


        #region Roles
        [HttpPost]
        public List<KeyValue> Get_Role_List_KeyValue()
        {
            var res = User_Manager.Get_Role_List_KeyValue();
            return res;
        }
        [HttpPost]
        public List<Roles_Model> Get_Roles_List()
        {
            var res = User_Manager.Get_Roles_List();
            return res;
        }
        [HttpPost]
        public Main_Roles_Model Get_Roles_ByID(dynamic obj)
        {
            var res = User_Manager.Get_Roles_ByID((long)obj.RoleID);
            return res;
        }
        [HttpPost]
        public long Roles_Update(dynamic obj)
        {
            var res = User_Manager.Roles_Update(obj.model.ToObject<Roles_Model>(), obj.Permission_List.ToObject<List<RolePermission_Model>>());
            Update_Page_Permission_Refresh(); //Refresh page permission in all login
            return res;
        }
        [HttpPost]
        public int Roles_Delete(dynamic obj)
        {
            var res = User_Manager.Roles_Delete((string)obj.RoleIDs);
            return res;
        }
        #endregion


        #endregion

        public void Update_Page_Permission_Refresh()
        {
            try
            {
                object obj = new { UserID = ClaimsModel.UserId };
            }
            catch (Exception) { }
        }
    }

}