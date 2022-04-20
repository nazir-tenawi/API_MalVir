using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Model
{
    public class Outgoing_Server_Model
    {        
        public string host { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public int port { get; set; }
        public bool enableSsl { get; set; }
        public bool defaultCredentials { get; set; }

        //appsetting
        public string fromEmail { get; set; }
        public string fromEmailAlias { get; set; }
    }
}
