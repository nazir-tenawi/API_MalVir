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

namespace MalVirDetector_CLI_API.Logic
{
    public class History_Manager
    {

        public static List<History_Model> Get_History_List(bool Is_Agent, bool Is_Client, long UserID)
        {
            List<History_Model> res = new List<History_Model>();
            try
            {
                SP_Get_History_List sp = new SP_Get_History_List() { Is_Agent = Is_Agent, Is_Client = Is_Client, UserID = UserID };
                res = DataManager.ExecuteSPGetList<History_Model, SP_Get_History_List>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static History_Model Get_History_ByID(long HistoryID)
        {
            History_Model res = new History_Model();
            try
            {
                SP_Get_History_ByID sp = new SP_Get_History_ByID() { HistoryID = HistoryID };
                res = DataManager.ExecuteSPGetSingle<History_Model, SP_Get_History_ByID>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static Tuple<long, string> History_Create(Scan_Search_File_Model model,long UserID, int ScanTypeID)
        {
            Tuple<long, string> res = null;
            try
            {
                SP_History_Create sp = new SP_History_Create();
                ComUti.MapObject(model, sp); //Assign model to sp class
                sp.CreatedDate = DateTime.Now;
                sp.CreatedUser = UserID;
                sp.UpdatedUser = UserID;
                sp.UpdatedDate = DateTime.Now;
                sp.ScanTypeID = ScanTypeID;



                DataManager.ExecuteSPNonQeury(sp); //Call SP
                long HistoryID = sp.HistoryID;
                res = new Tuple<long, string>(HistoryID, "");

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static Tuple<long, string> History_Update(History_Model model, long UserID)
        {
            Tuple<long, string> res = null;
            try
            {
                SP_History_Update sp = new SP_History_Update();
                ComUti.MapObject(model, sp); //Assign model to sp class
                sp.CreatedUser = UserID;
                sp.UpdatedUser = UserID;
                sp.UpdatedDate = DateTime.Now;
                DataManager.ExecuteSPNonQeury(sp); //Call SP
                long HistoryID = sp.HistoryID;
                res = new Tuple<long, string>(HistoryID, "");


            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        public static long History_Delete(string HistoryIDs)
        {
            long res = 0;
            try
            {
                SP_History_Delete sp = new SP_History_Delete() { HistoryIDs = HistoryIDs };
                res = DataManager.ExecuteSPNonQeury(sp);


            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }


        public static Tuple<long, string> Scan_History_Create(Hash_Model model, long UserID)
        {
            Tuple<long, string> res = null;
            History_Model history = new History_Model();
            try
            {
                // history.HistoryID = null;
                history.HashContent = null;
                history.ScanTypeID = null;
                history.HashID = null;
                // history.CreatedUser = null;
                // history.UpdatedUser = null;
                // history.CreatedDate = null;
                // history.UpdatedDate = null;
                // history.Is_Active = null;




                
                SP_History_Create sp = new SP_History_Create();
                ComUti.MapObject(history, sp); //Assign model to sp class

                sp.CreatedDate = DateTime.Now;
                sp.CreatedUser = UserID;
                sp.UpdatedUser = UserID;
                sp.UpdatedDate = DateTime.Now;





                DataManager.ExecuteSPNonQeury(sp); //Call SP

                long HistoryID = sp.HistoryID;
                
                
                res = new Tuple<long, string>(HistoryID, sp.HashContent);

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

    }

}
