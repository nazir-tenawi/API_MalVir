using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    #region AgentSetting
    public class AgentSetting_Model
    {
        public long AgentSettingID { get; set; }
        public bool Is_Profile_Visible { get; set; }
        public bool Is_CommonSetting_Visible { get; set; }
        public bool Is_History_Search { get; set; }
    }
    #endregion

    #region ClientSetting
    public class ClientSetting_Model
    {
        public long ClientSettingID { get; set; }
        public bool Is_Profile_Visible { get; set; }
        public bool Is_History_Visible { get; set; }
        public bool Is_Search_Visible { get; set; }
        public int PageSize { get; set; }
        public bool Is_History_Search { get; set; }
    }
    #endregion

    #region ApplicationSetting
    public class ApplicationSetting_Model
    {
        public long ApplicationSettingID { get; set; }
        public bool Is_EasyAddOn_Visible { get; set; }
        public string DefaultLanguage { get; set; }
        public string DefaultPassword { get; set; }
        public bool Is_LockUser { get; set; }
        public string CompanyTitle { get; set; }
        public string CompanyLogo { get; set; }
        public IFormFile attachment { get; set; }
        public string LogoName { get; set; }
        public bool Is_Admin_Search { get; set; } //Commonn grid search in admin pages
    }
    #endregion
}
