using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class EditReportSubscriptionModel
    {
        public int subscriptionID { get; set; }
        public string subscriptionName { get; set; }
        public string reportName { get; set; }
        public string selectedGroupNames { get; set; }
        public string selectedGroupIDs { get; set; }
        public Dictionary<string, string> dynamicParams { get; set; }
        public List<string> groupNames { get; set; }
        public List<string> groupIDs { get; set; }
    }
}
