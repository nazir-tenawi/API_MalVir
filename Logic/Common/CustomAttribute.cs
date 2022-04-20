using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalVirDetector_CLI_API.Logic
{
    public class SqlProcedureAttribute : Attribute
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SqlParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public bool IsOutput { get; set; }
        public bool NotBound { get; set; }
    }

    public class PropertyBinding : Attribute
    {
        public string Name { get; set; }
    }


    public class Claim_Types
    {
        public const string UserName = "UserName";
        public const string DisplayName = "DisplayName";
        public const string Email = "Email";
        public const string type = "type";
        public const string Is_Admin = "Is_Admin";
        public const string Is_Agent = "Is_Agent";
        public const string Is_Client = "Is_Client";
        public const string access_token = "access_token";
    }
    public class KeyValue
    {
        public string Key { get; set; }
        public long Value { get; set; }
    }
    public class KeyValueString
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class KeyValueDefault
    {
        public string Key { get; set; }
        public long Value { get; set; }
        public long MainID { get; set; }
    }

}
