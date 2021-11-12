using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Models
{
    public class AdminPageModel
    {
        public List<GroupModel> groupsList { get; set; }
        public List<string> masterGroupsList { get; set; }
    }
}
