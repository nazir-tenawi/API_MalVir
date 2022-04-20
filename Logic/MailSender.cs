// using MalVirDetector_CLI_API.Web.Helpers;
// using Microsoft.Extensions.Configuration;
// using System;
// using System.Collections.Generic;
// using System.Configuration;
// using System.Linq;
// using System.Net.Mail;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;

// namespace MalVirDetector_CLI_API.Logic
// {
//     public class MailSender
//     {
//         private static IConfiguration _config;
//         public MailSender(IConfiguration config)
//         {
//             _config = config;
//         }
//         public static int SendMail(string Subject, string Body, string ToEmail, string CC = "", string BCC = "")
//         {
//             int res = 0;
//             try
//             {
//                 using (MailMessage mailmsg = new MailMessage())
//                 {
//                     SMPTSetting setting = HtmlHelpers.ClaimsModelService.getSMTP();

//                     string fromMailAlias = setting.fromEmailAlias;// ComUti.GetAppSetting("fromEmailAlias");
//                     string fromMail = setting.Username;// ComUti.GetAppSetting("fromEmail");

//                     mailmsg.To.Add(ToEmail);
//                     //mailmsg.From = new MailAddress(setting.Username);
//                     mailmsg.Subject = Subject;
//                     mailmsg.Body = Body;
//                     mailmsg.IsBodyHtml = true;
//                     if (!string.IsNullOrEmpty(fromMailAlias) && !string.IsNullOrEmpty(fromMail)) { mailmsg.From = new MailAddress("\"" + fromMailAlias + "\"" + " " + fromMail); }
//                     else if (!string.IsNullOrEmpty(fromMail)) { mailmsg.From = new MailAddress(fromMail); }

//                     //SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort);
//                     //smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
//                     //smtp.EnableSsl = EnableSsl;
//                     //smtp.UseDefaultCredentials = SmtpUseDefaultCredentials;

//                     //SmtpClient smtp = new SmtpClient();
//                     //smtp.Timeout = 1500000;
//                     //smtp.Send(mailmsg);

//                     Thread thread = new Thread(delegate ()
//                     {
//                         SendSMTP(mailmsg, setting);
//                     });
//                     thread.IsBackground = true;
//                     thread.Start();
//                     res = 1;

//                 }
//             }
//             catch (Exception ex)
//             {
//                 Logger.Log(ex);
//             }
//             return res;
//         }


//         public static void SendSMTP(MailMessage mailmsg, SMPTSetting setting)
//         {
//             try
//             {
//                 using (SmtpClient smtp = new SmtpClient(setting.SmtpServer, setting.SmtpPort))
//                 {
//                     smtp.UseDefaultCredentials = false;
//                     smtp.Credentials = new System.Net.NetworkCredential(setting.Username, setting.Password);
//                     smtp.EnableSsl = setting.EnableSsl;
//                     smtp.Timeout = 1500000;
//                     smtp.Send(mailmsg);
//                 }

//                 //SmtpClient smtp = new SmtpClient(setting.SmtpServer, setting.SmtpPort != null ? Convert.ToInt32(setting.SmtpPort) : 587);
//                 //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
//                 //smtp.Credentials = new System.Net.NetworkCredential(setting.Username, setting.Password);
//                 //smtp.EnableSsl = setting.EnableSsl != null ? Convert.ToBoolean(setting.EnableSsl) : true;
//                 ////smtp.UseDefaultCredentials = SmtpUseDefaultCredentials;

//                 //smtp.Timeout = 1500000;
//                 //smtp.Send(mailmsg);
//             }
//             catch (Exception ex)
//             {
//                 mailmsg.Dispose();
//                 Logger.Log(ex);
//             }
//         }

//     }
// }
