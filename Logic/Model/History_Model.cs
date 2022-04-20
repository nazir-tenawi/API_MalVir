using MalVirDetector_CLI_API.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    public class History_Model
    {
        public long HistoryID { get; set; }
        public long? ScanTypeID { get; set; }
        public long? HashID { get; set; }
        public long CreatedUser { get; set; }
        public long UpdatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ScanTypeName { get; set; }
        public string HashContent { get; set; }
        public string CreatedUserName { get; set; }
    }
    public class File_Attachments
    {
        public string name { get; set; }
        public string type { get; set; }
        public string extension { get; set; }
        public string size { get; set; }
        public string value { get; set; }
    }

}

