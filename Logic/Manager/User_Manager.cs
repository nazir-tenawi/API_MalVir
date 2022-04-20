using MalVirDetector_CLI_API.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MalVirDetector_CLI_API.Logic
{
    public class User_Manager
    {
        #region UserManagement
        public static int Check_UserName_Available(long UserID, string UserName)
        {
            int res = 0;
            try
            {
                SP_Check_UserName_Available sp = new SP_Check_UserName_Available() { UserID = UserID, UserName = UserName };
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ReturnValue;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static Page_Permission_Model Page_Permission(long UserID)
        {
            Page_Permission_Model res = new Page_Permission_Model();
            try
            {
                SP_Page_Permission sp = new SP_Page_Permission() { UserID = UserID };
                res = DataManager.ExecuteSPGetSingle<Page_Permission_Model, SP_Page_Permission>(sp);

                res.Is_Agent_Original = res.Is_Agent;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static int ForgotPassword(string UserName, string site_url)
        {
            int res = 0;
            try
            {
                SP_Get_Email_ByUserName sp = new SP_Get_Email_ByUserName() { UserName = UserName };
                string Email = DataManager.ExecuteSPGetSingle<string, SP_Get_Email_ByUserName>(sp);

                if (!string.IsNullOrEmpty(Email))
                {
                    //string Site_URL = asset_url;// ComUti.GetAppSetting("Site_URL");
                    //string body = File.ReadAllText(HostingEnvironment.MapPath("~/Template/ForgotPassword.html"));
                    string body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Template/ForgotPassword.html")));
                    body = body.Replace("##USERNAME##", UserName);

                    string Key = AESEncrytDecry.EncryptStringAES(UserName + "||" + Email);
                    string strEncrypt = Key.Replace("+", "###"); //Replace + to ###

                    string url = site_url + "/reset_password/" + HttpUtility.UrlEncode(strEncrypt);
                    body = body.Replace("##LINK##", url);

                    // res = MailSender.SendMail("Reset Password", body, Email);
                }
                else
                {
                    res = -1;//username not exists in system
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static string Decrypt_ForgotPassword(string Key)
        {
            string res = "";
            try
            {
                Key = HttpUtility.UrlDecode(Key).Replace("###", "+");//Replace ### to +     
                res = AESEncrytDecry.DecryptStringAES(Key);  //0-UserName||1-Email
                //string[] str = strDecrypt.Split(new string[] { "||" }, StringSplitOptions.None);
                //if (str.Length > 0)
                //{
                //    res = str[0];
                //    //string email = str[1];
                //}

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static int ResetPassword(string UserName, string Email, string Password)
        {
            int res = 0;
            try
            {
                SP_ResetPassword sp = new SP_ResetPassword()
                {
                    UserID = 0,
                    UserName = UserName,
                    Email = Email,
                    Password = AESEncrytDecry.EncryptStringAES(Password)
                };
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ReturnValue;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static int ChangePassword(long UserID, string Password)
        {
            int res = 0;
            try
            {
                SP_ResetPassword sp = new SP_ResetPassword()
                {
                    UserID = UserID,
                    UserName = "",
                    Email = "",
                    Password = AESEncrytDecry.EncryptStringAES(Password)
                };
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.ReturnValue;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static List<User_Account_Model> Get_UserManagement_List(bool Is_Agent, bool Is_Client = false)
        {
            List<User_Account_Model> res = new List<User_Account_Model>();
            try
            {
                SP_Get_UserManagement_List sp = new SP_Get_UserManagement_List() { Is_Agent = Is_Agent, Is_Client = Is_Client };
                res = DataManager.ExecuteSPGetList<User_Account_Model, SP_Get_UserManagement_List>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static List<User_Account_Model> Get_UserSelection_List(bool Is_Agent)
        {
            List<User_Account_Model> res = new List<User_Account_Model>();
            try
            {
                SP_Get_UserSelection_List sp = new SP_Get_UserSelection_List() { Is_Agent = Is_Agent };
                res = DataManager.ExecuteSPGetList<User_Account_Model, SP_Get_UserSelection_List>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static User_Account_Model Get_UserManagement_ByID(long UserID)
        {
            User_Account_Model res = new User_Account_Model();
            try
            {
                SP_Get_UserManagement_ByID sp = new SP_Get_UserManagement_ByID() { UserID = UserID };
                res = DataManager.ExecuteSPGetSingle<User_Account_Model, SP_Get_UserManagement_ByID>(sp);
                if (res != null)
                {
                    res.Password = AESEncrytDecry.DecryptStringAES(res.Password);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static long UserManagement_Update(User_Account_Model model)
        {
            long res = 0; bool Is_Insert = false;
            try
            {
                SP_UserManagement_Update sp = new SP_UserManagement_Update();
                ComUti.MapObject(model, sp);
                if (model != null)
                {
                    sp.Password = AESEncrytDecry.EncryptStringAES(model.Password);
                    Is_Insert = model.UserID > 0 ? false : true;
                }

                DataManager.ExecuteSPNonQeury(sp);
                res = sp.UserID;

                if (!string.IsNullOrEmpty(model.ProfilePictureName))
                {
                    //var folderPath = HostingEnvironment.MapPath("~/Profile/");
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Profile/"));
                    var extension = Path.GetExtension(model.ProfilePictureName);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = res + extension;// res = UserID
                    var path = folderPath + fileName;
                    string data = model.ProfilePicture.Substring(model.ProfilePicture.IndexOf(',') + 1);
                    Byte[] bytes = Convert.FromBase64String(data);
                    File.WriteAllBytes(path, bytes);

                    SP_Update_User_ProfilePic sp1 = new SP_Update_User_ProfilePic() { UserID = res, ProfilePicture = fileName };
                    var result = DataManager.ExecuteSPNonQeury(sp1);
                }

                //Send Mail
                if (Is_Insert && !string.IsNullOrEmpty(model.Email))
                {
                    try
                    {
                        string body = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Template/ResetPassword.html")));
                        body = body.Replace("##LOGOURL##", ClaimsModel.API_Url).Replace("##USERNAME##", model.DisplayName).Replace("##PASSWORD##", model.Password);
                        // MailSender.SendMail("Login Password", body, model.Email);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static int UserManagement_Delete(string UserIDs)
        {
            int res = 0;
            try
            {
                SP_UserManagement_Delete sp = new SP_UserManagement_Delete() { UserIDs = UserIDs };
                res = DataManager.ExecuteSPNonQeury(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static int ActiveDeActive_User(List<KeyValue> lstUsers)
        {
            int res = 0;
            try
            {
                foreach (var item in lstUsers)
                {
                    var isAct = Convert.ToBoolean(item.Key) ? false : true;
                    SP_ActiveDeActive_User sp = new SP_ActiveDeActive_User() { UserID = item.Value, Is_Active = isAct };
                    res = DataManager.ExecuteSPNonQeury(sp);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static int ResetDefaultPassword_User(string UserIDs, string DefaultPassword)
        {
            int res = 0;
            try
            {
                DefaultPassword = AESEncrytDecry.EncryptStringAES(DefaultPassword);
                SP_ResetDefaultPassword_User sp = new SP_ResetDefaultPassword_User() { UserIDs = UserIDs, DefaultPassword = DefaultPassword };
                res = DataManager.ExecuteSPNonQeury(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        #endregion

        #region User Roles
        public static List<KeyValue> Get_Role_List_KeyValue()
        {
            List<KeyValue> res = new List<KeyValue>();
            try
            {
                SP_Get_Role_List_KeyValue sp = new SP_Get_Role_List_KeyValue();
                res = DataManager.ExecuteSPGetList<KeyValue, SP_Get_Role_List_KeyValue>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static List<Roles_Model> Get_Roles_List()
        {
            List<Roles_Model> res = new List<Roles_Model>();
            try
            {
                SP_Get_Roles_List sp = new SP_Get_Roles_List();
                res = DataManager.ExecuteSPGetList<Roles_Model, SP_Get_Roles_List>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static Main_Roles_Model Get_Roles_ByID(long RoleID)
        {
            Main_Roles_Model res = new Main_Roles_Model();
            try
            {
                SP_Get_Roles_ByID sp = new SP_Get_Roles_ByID() { RoleID = RoleID };
                var result = DataManager.ExecuteSPGetList<Roles_Model, RolePermission_Model, SP_Get_Roles_ByID>(sp);
                if (result != null)
                {
                    res.Roles_Model = result.Item1.Count > 0 ? result.Item1[0] : new Roles_Model();
                    res.RolePermission_Model = result.Item2.Count > 0 ? result.Item2 : new List<RolePermission_Model>();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static long Roles_Update(Roles_Model model, List<RolePermission_Model> Permission_List)
        {
            long res = 0;
            try
            {
                SP_Roles_Update sp = new SP_Roles_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.RoleID;

                if (res > 0)
                {
                    foreach (var Permission in Permission_List)
                    {
                        SP_RolePermission_Update sp1 = new SP_RolePermission_Update();
                        ComUti.MapObject(Permission, sp1);
                        sp1.RoleID = res;
                        DataManager.ExecuteSPNonQeury(sp1);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static int Roles_Delete(string RoleIDs)
        {
            int res = 0; bool isError = false;
            try
            {
                foreach (var roleID in RoleIDs.Split(","))
                {
                    //SP_Roles_Delete sp = new SP_Roles_Delete() { RoleIDs = RoleIDs };
                    //res = DataManager.ExecuteSPNonQeury(sp);
                    SP_Roles_Delete sp = new SP_Roles_Delete() { RoleIDs = roleID };
                    res = DataManager.ExecuteSPNonQeury(sp);
                    if (res <= 0) { isError = true; }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            if (isError) { res = 0; }
            return res;
        }


        #endregion


        #region Requester
        public static long Requester_UserManagement_Update(User_Account_Model model)
        {
            long res = 0;
            try
            {
                SP_Requester_UserManagement_Update sp = new SP_Requester_UserManagement_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.UserID;

                if (!string.IsNullOrEmpty(model.ProfilePictureName))
                {
                    //var folderPath = HostingEnvironment.MapPath("~/Profile/");
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Profile/"));
                    var extension = Path.GetExtension(model.ProfilePictureName);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = res + extension;// res = UserID
                    var path = folderPath + fileName;
                    string data = model.ProfilePicture.Substring(model.ProfilePicture.IndexOf(',') + 1); //a,b,c,d
                    Byte[] bytes = Convert.FromBase64String(data);
                    File.WriteAllBytes(path, bytes);

                    SP_Update_User_ProfilePic sp1 = new SP_Update_User_ProfilePic() { UserID = res, ProfilePicture = fileName };
                    var result = DataManager.ExecuteSPNonQeury(sp1);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        #endregion


        public static List<KeyValueString> Get_Languages()
        {
            List<KeyValueString> res = new List<KeyValueString>();
            try
            {
                res.Add(new KeyValueString { Key = "English", Value = "EN" });
                res.Add(new KeyValueString { Key = "French", Value = "FR" });
                res.Add(new KeyValueString { Key = "German", Value = "DE" });
                res.Add(new KeyValueString { Key = "Dutch", Value = "NL" });
                res.Add(new KeyValueString { Key = "Greek", Value = "GR" });
                res.Add(new KeyValueString { Key = "Swedish", Value = "SE" });
                res.Add(new KeyValueString { Key = "Spanish", Value = "ES" });
                //res.Add(new KeyValueString { Key = "Hebrew", Value = "HE" });
                res.Add(new KeyValueString { Key = "Japanese", Value = "JP" });
                res.Add(new KeyValueString { Key = "Danish", Value = "DK" });
                res.Add(new KeyValueString { Key = "Russian", Value = "RU" });
                res.Add(new KeyValueString { Key = "Chinese", Value = "TW" });
                res.Add(new KeyValueString { Key = "Portuguese", Value = "PT" });
                res.Add(new KeyValueString { Key = "Polish", Value = "PL" });
                res.Add(new KeyValueString { Key = "Italian", Value = "IT" });
                res.Add(new KeyValueString { Key = "Hungarian", Value = "HU" });
                res.Add(new KeyValueString { Key = "Turkish", Value = "TR" });
                res.Add(new KeyValueString { Key = "Arabic", Value = "AE" });

                res = res.OrderBy(d => d.Key).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

    }
}
