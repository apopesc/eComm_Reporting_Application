using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    //This model will contain all the data for each entry in the subscriptions groups table
    public class SubscriptionGroupsTableModel
    {
        public string userEmail { get; set; }
        public string isActive { get; set; }
        public string group { get; set; }
        public string groupID { get; set; }
        public string masterGroup { get; set; }
    }
}
