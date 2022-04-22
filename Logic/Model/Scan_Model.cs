using MalVirDetector_CLI_API.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    public class Scan_Model
    {
        public long HashID { get; set; }
        public string Type { get; set; }
        public string HashContent { get; set; }
        public DateTime? CreatedDate { get; set; }
        public decimal Size { get; set; }
    }



}
