using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class ReportTableModel
    {
        public int subscriptionID { get; set; }
        public string subscriptionName { get; set; }
        public string reportName { get; set; }
        public string groupName { get; set; }
        public string groupID { get; set; }

        public Dictionary<string,string> dynamicParams { get; set; }
    }
}
