using MalVirDetector_CLI_API.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Logic
{
    public class General_Setting_Manager
    {
        #region AgentSetting
        public static AgentSetting_Model Get_AgentSetting()
        {
            AgentSetting_Model res = new AgentSetting_Model();
            try
            {
                SP_Get_AgentSetting sp = new SP_Get_AgentSetting();
                res = DataManager.ExecuteSPGetSingle<AgentSetting_Model, SP_Get_AgentSetting>(sp);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static long AgentSetting_Update(AgentSetting_Model model)
        {
            long res = 0;
            try
            {
                SP_AgentSetting_Update sp = new SP_AgentSetting_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.AgentSettingID;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        #endregion

        #region ClientSetting
        public static ClientSetting_Model Get_ClientSetting()
        {
            ClientSetting_Model res = new ClientSetting_Model();
            try
            {
                SP_Get_ClientSetting sp = new SP_Get_ClientSetting();
                res = DataManager.ExecuteSPGetSingle<ClientSetting_Model, SP_Get_ClientSetting>(sp);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static long ClientSetting_Update(ClientSetting_Model model)
        {
            long res = 0;
            try
            {
                SP_ClientSetting_Update sp = new SP_ClientSetting_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ClientSettingID;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        #endregion

        #region ApplicationSetting
        public static ApplicationSetting_Model Get_ApplicationSetting()
        {
            ApplicationSetting_Model res = new ApplicationSetting_Model();
            try
            {
                SP_Get_ApplicationSetting sp = new SP_Get_ApplicationSetting();
                res = DataManager.ExecuteSPGetSingle<ApplicationSetting_Model, SP_Get_ApplicationSetting>(sp);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static long ApplicationSetting_Update(ApplicationSetting_Model model)
        {
            long res = 0;
            try
            {
                SP_ApplicationSetting_Update sp = new SP_ApplicationSetting_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ApplicationSettingID;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static long Logo_Update(ApplicationSetting_Model model)
        {
            long res = 0;
            try
            {
                string fileName = "";
                if (!string.IsNullOrEmpty(model.LogoName))
                {
                    //var folderPath = HostingEnvironment.MapPath("~/Attachments/");
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Attachments/"));
                    var extension = Path.GetExtension(model.LogoName);
                    fileName = "logo" + extension;
                    var path = folderPath + fileName;
                    string data = model.CompanyLogo.Substring(model.CompanyLogo.IndexOf(',') + 1); //a,b,c,d
                    Byte[] bytes = Convert.FromBase64String(data);
                    File.WriteAllBytes(path, bytes);
                }

                SP_Logo_Update sp = new SP_Logo_Update()
                {
                    ApplicationSettingID = model.ApplicationSettingID,
                    CompanyLogo = fileName,
                    CompanyTitle = model.CompanyTitle
                };

                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ApplicationSettingID;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static string Get_DefaultLang()
        {
            string res = "EN";
            try
            {
                // SP_Get_ApplicationSetting sp = new SP_Get_ApplicationSetting();
                // var result = DataManager.ExecuteSPGetSingle<ApplicationSetting_Model, SP_Get_ApplicationSetting>(sp);
                // if (result != null) { res = result.DefaultLanguage; }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        #endregion

    }
}
