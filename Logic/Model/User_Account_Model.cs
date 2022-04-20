using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    public class User_Account_Model
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int? TimeZoneID { get; set; }
        public bool? Is_SendMail_Password { get; set; }
        public string Description { get; set; }
        public string ProfilePicture { get; set; }
        public string ProfilePictureName { get; set; }
        //public string ProfilePicture_base64 { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_Agent { get; set; }
        public bool Is_Client { get; set; }
    }
    public class Main_Roles_Model {
        public Roles_Model Roles_Model { get; set; }
        public List<RolePermission_Model> RolePermission_Model { get; set; }
    }
    public class Roles_Model
    {
        public long RoleID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Is_Agent { get; set; }
        public bool Is_Client { get; set; }
        public bool Is_Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class RolePermission_Model
    {
        public long RolePermissionID { get; set; }
        public long RoleID { get; set; }
        public long MenuID { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }
        public bool Is_Full { get; set; }
        public bool Is_View { get; set; }
        public bool Is_Add { get; set; }
        public bool Is_Edit { get; set; }
        public bool Is_Delete { get; set; }
        public bool Is_Active { get; set; }
        //public DateTime CreatedDate { get; set; }
    }
    public class Page_Permission_Model
    {
        public long UserID { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public bool Is_Agent_Original { get; set; }
        public bool Is_Agent { get; set; }
        public bool Is_Client { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyTitle { get; set; }
        public string DefaultLanguage { get; set; }
        public string DefaultPassword { get; set; }
        public bool Is_Admin_Search { get; set; }        
        public bool Is_History_StartPage { get; set; }


        public bool Is_Profile_Visible { get; set; }
        public bool Is_Profile_Visible_Client { get; set; }
        public bool Is_CommonSetting_Visible { get; set; }
        public bool Is_CommonSetting_Visible_Client { get; set; }

        public int PageSize { get; set; }
        public int PageSize_Client { get; set; }
        public bool Is_History_Visible_Client { get; set; }
        public bool Is_Search_Visible_Client { get; set; }

        public bool Is_History_Search { get; set; }
        public bool Is_History_Search_Client { get; set; }
        public bool Is_Full_History { get; set; }
        public bool Is_View_History { get; set; }
        public bool Is_Add_History { get; set; }
        public bool Is_Edit_History { get; set; }
        public bool Is_Delete_History { get; set; }

        public bool Is_Full_History_Client { get; set; }
        public bool Is_View_History_Client { get; set; }
        public bool Is_Add_History_Client { get; set; }
        public bool Is_Edit_History_Client { get; set; }
        public bool Is_Delete_History_Client { get; set; }



        public bool Is_Full_Admin { get; set; }
        public bool Is_View_Admin { get; set; }
        public bool Is_Add_Admin { get; set; }
        public bool Is_Edit_Admin { get; set; }
        public bool Is_Delete_Admin { get; set; }

    }
}
