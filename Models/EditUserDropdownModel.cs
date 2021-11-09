using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class EditUserDropdownModel
    {
        public int ID { get; set; }
        public string userEmail { get; set; }
        public string isActive { get; set; }
        public List<string> masterGroupsList { get; set; }
        public string selectedGroupNames { get; set; }
        public string selectedMasterGroups { get; set; }
        public string selectedGroupIDs { get; set; }
    }
}
