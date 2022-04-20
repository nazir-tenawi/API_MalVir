using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    #region ScanType
    public class ScanType_Model
    {
        public long ScanTypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Is_Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    #endregion

    #region Hash
    public class Hash_Model
    {
        public long HashID { get; set; }
        public string Type { get; set; }
        public string HashContent { get; set; }
        public decimal Size { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    #endregion
}
