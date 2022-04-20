using MalVirDetector_CLI_API.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Logic
{



    #region Scan
    [SqlProcedure(Name = "Scan_Search")]
    public class SP_Scan_Search
    {
        [SqlParameter(Name = "HashContent")]
        public string HashContent { get; set; }
    }

    #endregion
    #region UserManagement
    [SqlProcedure(Name = "Login_User")]
    public class SP_Login_User
    {
        [SqlParameter(Name = "UserName")]
        public string UserName { get; set; }
        [SqlParameter(Name = "Password")]
        public string Password { get; set; }
    }

    [SqlProcedure(Name = "Page_Permission")]
    public class SP_Page_Permission
    {
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
    }


    #region User
    [SqlProcedure(Name = "Check_UserName_Available")]
    public class SP_Check_UserName_Available
    {
        [SqlParameter(Name = "ReturnValue", IsOutput = true)]
        public int ReturnValue { get; set; }
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
        [SqlParameter(Name = "UserName")]
        public string UserName { get; set; }
    }

    [SqlProcedure(Name = "Get_UserManagement_List")]
    public class SP_Get_UserManagement_List
    {
        [SqlParameter(Name = "Is_Agent")]
        public bool Is_Agent { get; set; }
        [SqlParameter(Name = "Is_Client")]
        public bool Is_Client { get; set; }
    }
    [SqlProcedure(Name = "Get_UserSelection_List")]
    public class SP_Get_UserSelection_List
    {
        [SqlParameter(Name = "Is_Agent")]
        public bool Is_Agent { get; set; }
        //[SqlParameter(Name = "Is_Active")]
        //public bool Is_Active { get; set; }
    }
    [SqlProcedure(Name = "Get_UserManagement_ByID")]
    public class SP_Get_UserManagement_ByID
    {
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
    }
    [SqlProcedure(Name = "UserManagement_Update")]
    public class SP_UserManagement_Update
    {
        [SqlParameter(Name = "UserID", IsOutput = true)]
        public long UserID { get; set; }
        [SqlParameter(Name = "RoleID")]
        public long RoleID { get; set; }
        [SqlParameter(Name = "DisplayName")]
        public string DisplayName { get; set; }
        [SqlParameter(Name = "UserName")]
        public string UserName { get; set; }
        [SqlParameter(Name = "Password")]
        public string Password { get; set; }
        [SqlParameter(Name = "Email")]
        public string Email { get; set; }
        [SqlParameter(Name = "PhoneNo")]
        public string PhoneNo { get; set; }
        [SqlParameter(Name = "TimeZoneID")]
        public int? TimeZoneID { get; set; }
        [SqlParameter(Name = "Is_SendMail_Password")]
        public bool Is_SendMail_Password { get; set; }
        [SqlParameter(Name = "Description")]
        public string Description { get; set; }
        [SqlParameter(Name = "Is_Active")]
        public bool Is_Active { get; set; }
    }
    [SqlProcedure(Name = "UserManagement_Delete")]
    public class SP_UserManagement_Delete
    {
        [SqlParameter(Name = "UserID")]
        public string UserIDs { get; set; }
    }
    [SqlProcedure(Name = "Update_User_ProfilePic")]
    public class SP_Update_User_ProfilePic
    {
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
        [SqlParameter(Name = "ProfilePicture")]
        public string ProfilePicture { get; set; }
    }

    [SqlProcedure(Name = "Get_Email_ByUserName")]
    public class SP_Get_Email_ByUserName
    {
        [SqlParameter(Name = "UserName")]
        public string UserName { get; set; }
    }

    [SqlProcedure(Name = "ResetPassword")]
    public class SP_ResetPassword
    {
        [SqlParameter(Name = "ReturnValue", IsOutput = true)]
        public int ReturnValue { get; set; }
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
        [SqlParameter(Name = "UserName")]
        public string UserName { get; set; }
        [SqlParameter(Name = "Email")]
        public string Email { get; set; }
        [SqlParameter(Name = "Password")]
        public string Password { get; set; }
    }


    [SqlProcedure(Name = "ActiveDeActive_User")]
    public class SP_ActiveDeActive_User
    {
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
        [SqlParameter(Name = "Is_Active")]
        public bool Is_Active { get; set; }
    }

    [SqlProcedure(Name = "ResetDefaultPassword_User")]
    public class SP_ResetDefaultPassword_User
    {
        [SqlParameter(Name = "UserID")]
        public string UserIDs { get; set; }
        [SqlParameter(Name = "DefaultPassword")]
        public string DefaultPassword { get; set; }
    }


    #region Requester Profile
    [SqlProcedure(Name = "Requester_UserManagement_Update")]
    public class SP_Requester_UserManagement_Update
    {
        [SqlParameter(Name = "UserID", IsOutput = true)]
        public long UserID { get; set; }
        [SqlParameter(Name = "DisplayName")]
        public string DisplayName { get; set; }
        [SqlParameter(Name = "Email")]
        public string Email { get; set; }
        [SqlParameter(Name = "PhoneNo")]
        public string PhoneNo { get; set; }
        [SqlParameter(Name = "TimeZoneID")]
        public int? TimeZoneID { get; set; }
        [SqlParameter(Name = "Description")]
        public string Description { get; set; }
    }

    #endregion


    #endregion

    #region User Roles

    [SqlProcedure(Name = "Get_Role_List_KeyValue")]
    public class SP_Get_Role_List_KeyValue
    {
    }
    [SqlProcedure(Name = "Get_Roles_List")]
    public class SP_Get_Roles_List
    {
    }
    [SqlProcedure(Name = "Get_Roles_ByID")]
    public class SP_Get_Roles_ByID
    {
        [SqlParameter(Name = "RoleID")]
        public long RoleID { get; set; }
    }
    [SqlProcedure(Name = "Roles_Update")]
    public class SP_Roles_Update
    {
        [SqlParameter(Name = "RoleID", IsOutput = true)]
        public long RoleID { get; set; }
        [SqlParameter(Name = "Name")]
        public string Name { get; set; }
        [SqlParameter(Name = "Description")]
        public string Description { get; set; }
        [SqlParameter(Name = "Is_Agent")]
        public bool Is_Agent { get; set; }
        [SqlParameter(Name = "Is_Client")]
        public bool Is_Client { get; set; }
        [SqlParameter(Name = "Is_Active")]
        public bool Is_Active { get; set; }
    }
    [SqlProcedure(Name = "Roles_Delete")]
    public class SP_Roles_Delete
    {
        [SqlParameter(Name = "RoleID")]
        public string RoleIDs { get; set; }
    }

    [SqlProcedure(Name = "RolePermission_Update")]
    public class SP_RolePermission_Update
    {
        [SqlParameter(Name = "RolePermissionID", IsOutput = true)]
        public long RolePermissionID { get; set; }
        [SqlParameter(Name = "RoleID")]
        public long RoleID { get; set; }
        [SqlParameter(Name = "MenuID")]
        public long MenuID { get; set; }
        [SqlParameter(Name = "Is_Full")]
        public bool Is_Full { get; set; }
        [SqlParameter(Name = "Is_View")]
        public bool Is_View { get; set; }
        [SqlParameter(Name = "Is_Add")]
        public bool Is_Add { get; set; }
        [SqlParameter(Name = "Is_Edit")]
        public bool Is_Edit { get; set; }
        [SqlParameter(Name = "Is_Delete")]
        public bool Is_Delete { get; set; }
        [SqlParameter(Name = "Is_Active")]
        public bool Is_Active { get; set; }
    }
    #endregion


    #endregion
    #region Admin Basic
    #region Hash
    [SqlProcedure(Name = "Get_Hash_List")]
    public class SP_Get_Hash_List
    {
    }
    [SqlProcedure(Name = "Get_Hash_ByID")]
    public class SP_Get_Hash_ByID
    {
        [SqlParameter(Name = "HashID")]
        public long HashID { get; set; }
    }
    [SqlProcedure(Name = "Hash_Update")]
    public class SP_Hash_Update
    {
        [SqlParameter(Name = "HashID", IsOutput = true)]
        public long HashID { get; set; }

        [SqlParameter(Name = "Type")]
        public string Type { get; set; }
        [SqlParameter(Name = "HashContent")]
        public string HashContent { get; set; }
        [SqlParameter(Name = "Size")]
        public decimal Size { get; set; }
    }
    [SqlProcedure(Name = "Hash_Delete")]
    public class SP_Hash_Delete
    {
        [SqlParameter(Name = "HashID")]
        public string HashIDs { get; set; }
    }
    #endregion
    #endregion
    #region Common Setting
    [SqlProcedure(Name = "Get_AgentSetting")]
    public class SP_Get_AgentSetting
    { }
    [SqlProcedure(Name = "AgentSetting_Update")]
    public class SP_AgentSetting_Update
    {
        [SqlParameter(Name = "AgentSettingID", IsOutput = true)]
        public long AgentSettingID { get; set; }
        [SqlParameter(Name = "Is_Profile_Visible")]
        public bool Is_Profile_Visible { get; set; }
        [SqlParameter(Name = "Is_CommonSetting_Visible")]
        public bool Is_CommonSetting_Visible { get; set; }
        [SqlParameter(Name = "Is_Help_Visible")]
        public bool Is_Help_Visible { get; set; }
        [SqlParameter(Name = "Is_Solution_Visible")]
        public bool Is_Solution_Visible { get; set; }
        [SqlParameter(Name = "Is_ColumnChooser_Visible")]
        public bool Is_ColumnChooser_Visible { get; set; }
        [SqlParameter(Name = "PageSize")]
        public int PageSize { get; set; }

        [SqlParameter(Name = "Is_Print")]
        public bool Is_Print { get; set; }
        [SqlParameter(Name = "Is_Export")]
        public bool Is_Export { get; set; }
        [SqlParameter(Name = "Is_History_Search")]
        public bool Is_History_Search { get; set; }
        [SqlParameter(Name = "Is_Solution_Search")]
        public bool Is_Solution_Search { get; set; }
        [SqlParameter(Name = "Is_Column_Filter_History")]
        public bool Is_Column_Filter_History { get; set; }
        [SqlParameter(Name = "Is_Column_Filter_Solution")]
        public bool Is_Column_Filter_Solution { get; set; }
        [SqlParameter(Name = "Is_Clone_History")]
        public bool Is_Clone_History { get; set; }
        [SqlParameter(Name = "Is_Clone_Solution")]
        public bool Is_Clone_Solution { get; set; }
    }

    [SqlProcedure(Name = "Get_ClientSetting")]
    public class SP_Get_ClientSetting
    { }
    [SqlProcedure(Name = "ClientSetting_Update")]
    public class SP_ClientSetting_Update
    {
        [SqlParameter(Name = "ClientSettingID", IsOutput = true)]
        public long ClientSettingID { get; set; }
        [SqlParameter(Name = "Is_Profile_Visible")]
        public bool Is_Profile_Visible { get; set; }
        [SqlParameter(Name = "Is_Help_Visible")]
        public bool Is_Help_Visible { get; set; }
        [SqlParameter(Name = "Is_History_Visible")]
        public bool Is_History_Visible { get; set; }
        [SqlParameter(Name = "Is_Solution_Visible")]
        public bool Is_Solution_Visible { get; set; }
        [SqlParameter(Name = "Is_ColumnChooser_Visible")]
        public bool Is_ColumnChooser_Visible { get; set; }
        [SqlParameter(Name = "Is_Search_Visible")]
        public bool Is_Search_Visible { get; set; }
        [SqlParameter(Name = "PageSize")]
        public int PageSize { get; set; }

        [SqlParameter(Name = "Is_Print")]
        public bool Is_Print { get; set; }
        [SqlParameter(Name = "Is_Export")]
        public bool Is_Export { get; set; }
        [SqlParameter(Name = "Is_History_Search")]
        public bool Is_History_Search { get; set; }
        [SqlParameter(Name = "Is_Solution_Search")]
        public bool Is_Solution_Search { get; set; }
        [SqlParameter(Name = "Is_Column_Filter_History")]
        public bool Is_Column_Filter_History { get; set; }
        [SqlParameter(Name = "Is_Column_Filter_Solution")]
        public bool Is_Column_Filter_Solution { get; set; }
        [SqlParameter(Name = "Is_Clone_History")]
        public bool Is_Clone_History { get; set; }
        [SqlParameter(Name = "Is_Clone_Solution")]
        public bool Is_Clone_Solution { get; set; }
    }


    [SqlProcedure(Name = "Get_ApplicationSetting")]
    public class SP_Get_ApplicationSetting
    { }
    [SqlProcedure(Name = "ApplicationSetting_Update")]
    public class SP_ApplicationSetting_Update
    {
        [SqlParameter(Name = "ApplicationSettingID", IsOutput = true)]
        public long ApplicationSettingID { get; set; }
        [SqlParameter(Name = "Is_EasyAddOn_Visible")]
        public bool Is_EasyAddOn_Visible { get; set; }
        [SqlParameter(Name = "DefaultLanguage")]
        public string DefaultLanguage { get; set; }
        [SqlParameter(Name = "DefaultPassword")]
        public string DefaultPassword { get; set; }
        [SqlParameter(Name = "Is_Chat_Visible")]
        public bool Is_Chat_Visible { get; set; }
        [SqlParameter(Name = "Is_LockUser")]
        public bool Is_LockUser { get; set; }
        [SqlParameter(Name = "Is_Admin_Search")]
        public bool Is_Admin_Search { get; set; }
        [SqlParameter(Name = "Is_Pickup")]
        public bool Is_Pickup { get; set; }
        [SqlParameter(Name = "Is_AssignTo_Dropdown")]
        public bool Is_AssignTo_Dropdown { get; set; }
        [SqlParameter(Name = "Is_Close_History")]
        public bool Is_Close_History { get; set; }
        [SqlParameter(Name = "Is_History_StartPage")]
        public bool Is_History_StartPage { get; set; }
        [SqlParameter(Name = "Is_EditRow_On_DoubleClick")]
        public bool Is_EditRow_On_DoubleClick { get; set; }
    }
    [SqlProcedure(Name = "Logo_Update")]
    public class SP_Logo_Update
    {
        [SqlParameter(Name = "ApplicationSettingID", IsOutput = true)]
        public long ApplicationSettingID { get; set; }
        [SqlParameter(Name = "CompanyLogo")]
        public string CompanyLogo { get; set; }
        [SqlParameter(Name = "CompanyTitle")]
        public string CompanyTitle { get; set; }
    }


    #endregion
    #region History
    [SqlProcedure(Name = "Get_History_List")]
    public class SP_Get_History_List
    {
        [SqlParameter(Name = "Is_Agent")]
        public bool Is_Agent { get; set; }
        [SqlParameter(Name = "Is_Client")]
        public bool Is_Client { get; set; }
        [SqlParameter(Name = "UserID")]
        public long UserID { get; set; }
    }
    [SqlProcedure(Name = "Get_History_ByID")]
    public class SP_Get_History_ByID
    {
        [SqlParameter(Name = "HistoryID")]
        public long HistoryID { get; set; }
    }

    [SqlProcedure(Name = "History_Create")]
    public class SP_History_Create
    {
       [SqlParameter(Name = "HistoryID", IsOutput = true)]
        public long HistoryID { get; set; }
        [SqlParameter(Name = "ScanTypeID")]
        public long ScanTypeID { get; set; }
        [SqlParameter(Name = "HashID")]
        public long HashID { get; set; }
        [SqlParameter(Name = "CreatedUser")]
        public long CreatedUser { get; set; }
        [SqlParameter(Name = "UpdatedUser")]
        public long UpdatedUser { get; set; }
        [SqlParameter(Name = "UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [SqlParameter(Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [SqlParameter(Name = "HashContent")]
        public String HashContent { get; set; }
    }


    [SqlProcedure(Name = "History_Update")]
    public class SP_History_Update
     {
       [SqlParameter(Name = "HistoryID", IsOutput = true)]
        public long HistoryID { get; set; }

        [SqlParameter(Name = "ScanTypeID")]
        public long ScanTypeID { get; set; }
        [SqlParameter(Name = "HashID")]
        public long HashID { get; set; }
        [SqlParameter(Name = "CreatedUser")]
        public long CreatedUser { get; set; }
        [SqlParameter(Name = "UpdatedUser")]
        public long UpdatedUser { get; set; }
        [SqlParameter(Name = "UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [SqlParameter(Name = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }
        [SqlParameter(Name = "HashContent")]
        public String HashContent { get; set; }
    }

    [SqlProcedure(Name = "History_Delete")]
    public class SP_History_Delete
    {
        [SqlParameter(Name = "HistoryID")]
        public string HistoryIDs { get; set; }
    }


    [SqlProcedure(Name = "Get_History_Detail_Data")]
    public class SP_Get_History_Detail_Data
    {
        [SqlParameter(Name = "Is_Agent")]
        public bool Is_Agent { get; set; }
    }
    [SqlProcedure(Name = "Get_DescriptionByID")]
    public class SP_Get_DescriptionByID
    {
        [SqlParameter(Name = "ModuleType")]
        public string ModuleType { get; set; }
        [SqlParameter(Name = "ID")]
        public string ID { get; set; }
    }

    #endregion

}

