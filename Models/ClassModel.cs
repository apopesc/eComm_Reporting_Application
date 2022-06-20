using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class ClassModel
    {
        public ReportModel reportData { get; set; }
        public List<string> selectedDepartments { get; set; }
        public List<string> selectedClasses { get; set; }
        public string viewBy { get; set; }
    }
}
