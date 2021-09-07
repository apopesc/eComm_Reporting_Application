using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class ReportParameterModel
    {
        public string reportName { get; set; }
        public List<string> parameters { get; set; }
    }
}
