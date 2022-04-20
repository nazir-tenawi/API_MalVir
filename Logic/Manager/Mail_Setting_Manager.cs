using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MalVirDetector_CLI_API.Model;
using MalVirDetector_CLI_API.Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MalVirDetector_CLI_API.Logic
{
    public class Mail_Setting_Manager
    {
        #region Outgoing Email Server

        public static Outgoing_Server_Model Get_Outgoing_Server()
        {
            Outgoing_Server_Model model = new Outgoing_Server_Model();
            try
            {
                SMPTSetting setting = HtmlHelpers.ClaimsModelService.getSMTP();

                model.defaultCredentials = setting.DefaultCredentials;
                model.host = setting.SmtpServer;
                model.userName = setting.Username;
                model.password = setting.Password;
                model.port = setting.SmtpPort;
                model.enableSsl = setting.EnableSsl;
                model.fromEmail = !string.IsNullOrEmpty(setting.fromEmail) ? setting.fromEmail : setting.Username;
                model.fromEmailAlias = setting.fromEmailAlias;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return model;
        }

        public static int Update_Outgoing_Server(Outgoing_Server_Model model, string file_name)
        {
            int res = 0;
            try
            {
                if (model != null)
                {
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), file_name);

                    var json = File.ReadAllText(filepath);
                    dynamic jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                    jsonObj["smtp"]["fromEmailAlias"] = model.fromEmailAlias;
                    jsonObj["smtp"]["fromEmail"] = model.fromEmail;
                    jsonObj["smtp"]["Username"] = model.userName;
                    jsonObj["smtp"]["Password"] = model.password;
                    jsonObj["smtp"]["SmtpServer"] = model.host;
                    jsonObj["smtp"]["SmtpPort"] = model.port;
                    jsonObj["smtp"]["EnableSsl"] = model.enableSsl;
                    jsonObj["smtp"]["DefaultCredentials"] = model.defaultCredentials;

                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(filepath, output);


                    //HtmlHelpers.ClaimsModelService.UpdateSMTP("DefaultCredentials", model.defaultCredentials.ToString());
                    //Configuration webconfig = WebConfigurationManager.OpenWebConfiguration("~");
                    //SmtpSection smtp = (SmtpSection)(webconfig.SectionGroups["system.net"].SectionGroups["mailSettings"].Sections["smtp"]);
                    //SmtpNetworkElement Network = smtp.Network;
                    //smtp.From = model.fromEmail;
                    //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                    //Network.Host = model.host;
                    //Network.Port = model.port;
                    //Network.UserName = model.userName;
                    //Network.Password = model.password;
                    //Network.EnableSsl = model.enableSsl;
                    //Network.DefaultCredentials = model.defaultCredentials;

                    //webconfig.AppSettings.Settings.Remove("fromEmailAlias");
                    //webconfig.AppSettings.Settings.Add("fromEmailAlias", model.fromEmailAlias);

                    //webconfig.AppSettings.Settings.Remove("fromEmail");
                    //webconfig.AppSettings.Settings.Add("fromEmail", model.fromEmail);

                    //webconfig.Save(ConfigurationSaveMode.Full);
                    res = 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static int Test_Outgoing_Server(Outgoing_Server_Model model)
        {
            int res = 0;
            try
            {
                res = TestOutgoingEmailServer(model);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        #endregion






        #region SendMail
        public static int TestOutgoingEmailServer(Outgoing_Server_Model model)
        {
            int res = 0;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(model.fromEmail);
                    mail.To.Add(new MailAddress(model.fromEmail));
                    mail.Subject = "Test Outgoing Email Server Connection";
                    mail.Body = "<b>Test Outgoing Email Server connection Successfully.</b>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(model.host, model.port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential(model.userName, model.password);
                        smtp.EnableSsl = model.enableSsl;
                        smtp.Send(mail);
                    }
                    res = 1;
                }
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
