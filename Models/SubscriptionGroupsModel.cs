using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class SubscriptionGroupsModel
    {
        public bool isActive { get; set; }
        public List<string> groupsIDList { get; set; }
        public List<string> groupsList { get; set; }
        public List<string> masterGroupsList { get; set; }
    }
}
