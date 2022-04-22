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

        public static Scan_Model Scan_File(File_Attachments Attachments, long UserID)
        {
            Scan_Model res = new Scan_Model();
            try
            {
                var path = Save_Attachments(Attachments);
                var hashContent = CalculateMD5(path);
                SP_Scan_Search sp = new SP_Scan_Search();
                sp.HashContent = (string) hashContent;

                res = DataManager.ExecuteSPGetSingle<Scan_Model, SP_Scan_Search>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static Tuple<int, List<Scan_Model>> Scan_Wireshark(File_Attachments Attachments, long UserID)
        {
            List<Scan_Model> res2 = new List<Scan_Model>();
            Tuple<int, List<Scan_Model>> res = null;

            try
            {
                    string path = Save_Attachments(Attachments);
                    // string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Public\TestFolder\WriteLines2.txt");
                    string[] lines = System.IO.File.ReadAllLines(@path);
                    List<string> filteredList = lines.Where(x => x.IndexOf("[Frame MD5 Hash") > 0).ToList();
                    List<Scan_Model> TrimmedList = filteredList.Select(x => new Scan_Model() { HashContent = x.Substring(x.IndexOf(":") + 2 , x.IndexOf(":") + 13)}).ToList();
                    foreach (Scan_Model line in TrimmedList)
                    {
                        SP_Scan_Search sp = new SP_Scan_Search(){HashContent = (string) line.HashContent };
                        var hashRes = DataManager.ExecuteSPGetSingle<Scan_Model, SP_Scan_Search>(sp);
                        if (hashRes != null){
                            res2.Add(hashRes);
                        }
                    }
                    res = new Tuple<int, List<Scan_Model>>(TrimmedList.Count, res2);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static Scan_Model Scan_Search(string HashContent, long UserID)
        {
            Scan_Model res = new Scan_Model();
            try
            {

                SP_Scan_Search sp = new SP_Scan_Search();
                sp.HashContent = (string) HashContent;

                res = DataManager.ExecuteSPGetSingle<Scan_Model, SP_Scan_Search>(sp);
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

