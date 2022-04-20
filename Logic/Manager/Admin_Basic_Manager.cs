using MalVirDetector_CLI_API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Logic
{
    public class Admin_Basic_Manager
    {
        #region Admin Basic
        #region Hash
        public static List<Hash_Model> Get_Hash_List()
        {
            List<Hash_Model> res = new List<Hash_Model>();
            try
            {
                SP_Get_Hash_List sp = new SP_Get_Hash_List();
                res = DataManager.ExecuteSPGetList<Hash_Model, SP_Get_Hash_List>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static Hash_Model Get_Hash_ByID(long HashID)
        {
            Hash_Model res = new Hash_Model();
            try
            {
                SP_Get_Hash_ByID sp = new SP_Get_Hash_ByID() { HashID = HashID };
                res = DataManager.ExecuteSPGetSingle<Hash_Model, SP_Get_Hash_ByID>(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static long Hash_Update(Hash_Model model)
        {
            long res = 0;
            try
            {
                SP_Hash_Update sp = new SP_Hash_Update();
                ComUti.MapObject(model, sp);
                DataManager.ExecuteSPNonQeury(sp);
                res = sp.HashID;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }
        public static int Hash_Delete(string HashIDs)
        {
            int res = 0;
            try
            {
                SP_Hash_Delete sp = new SP_Hash_Delete() { HashIDs = HashIDs };
                res = DataManager.ExecuteSPNonQeury(sp);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return res;
        }

        #endregion


        #endregion

    }
}
