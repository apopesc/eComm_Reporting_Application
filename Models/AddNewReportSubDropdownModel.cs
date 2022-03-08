using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class AddNewReportSubDropdownModel
    {
        public List<ReportFolderModel> folders { get; set; }

        public List<string> groupNames { get; set; }
        public List<string> groupIDs { get; set; }

        public string selectedReport { get; set; }
        public string selectedFolder { get; set; }
    }
}
