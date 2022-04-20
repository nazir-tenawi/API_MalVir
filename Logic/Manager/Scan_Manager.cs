using MalVirDetector_CLI_API.Model;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


using System.IO;
using System.Security.Cryptography;

namespace MalVirDetector_CLI_API.Logic
{
    public class Scan_Manager
    {

        public static Scan_Search_File_Model Scan_File(File_Attachments Attachments, long UserID)
        {
            Scan_Search_File_Model res = new Scan_Search_File_Model();
            try
            {
                var path = Save_Attachments(Attachments);
                var hashContent = CalculateMD5(path);
                SP_Scan_Search sp = new SP_Scan_Search();
                sp.HashContent = (string) hashContent;

                res = DataManager.ExecuteSPGetSingle<Scan_Search_File_Model, SP_Scan_Search>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static Scan_Search_File_Model Scan_Wireshark(File_Attachments Attachments, long UserID)
        {
            Scan_Search_File_Model res = new Scan_Search_File_Model();
            try
            {
                    Save_Attachments(Attachments);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static Scan_Search_File_Model Scan_Search(string HashContent, long UserID)
        {
            Scan_Search_File_Model res = new Scan_Search_File_Model();
            try
            {

                SP_Scan_Search sp = new SP_Scan_Search();
                sp.HashContent = (string) HashContent;

                res = DataManager.ExecuteSPGetSingle<Scan_Search_File_Model, SP_Scan_Search>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        
        public static string Save_Attachments(File_Attachments item)
        {

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Documents", "Attachments/ScanFiles/"));
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

                var extension = Path.GetExtension(item.name);
                string fileName = DateTime.Now.Ticks.ToString() + extension;
                var path = folderPath +fileName;

                string data = item.value.Substring(item.value.IndexOf(',') + 1); //a,b,c,d
                Byte[] bytes = Convert.FromBase64String(data);
                File.WriteAllBytes(path, bytes);




               return path;
                
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

    }
}

