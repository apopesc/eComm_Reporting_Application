﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class ReportParameterModel
    {
        public string reportName { get; set; }
        public string dataSource { get; set; }
        //public List<DataSet> dataSets {get; set;}
        public List<Parameter> parameters { get; set; }
    }
}
