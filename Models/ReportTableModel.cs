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
        public string groupNames { get; set; }
        public string groupIDs { get; set; }
        public string fileFormat { get; set; }
        public string schedule { get; set; }

        public Dictionary<string,string> dynamicParams { get; set; }
    }
}
