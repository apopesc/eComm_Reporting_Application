using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class Parameter
    {
        public string name { get; set; }
        public string type { get; set; }
        public string queryType { get; set; }
        public string query { get; set; }
        public List<string> values { get; set; }
        public List<string> labels { get; set; }
    }
}
