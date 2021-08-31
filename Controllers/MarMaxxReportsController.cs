using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eComm_Reporting_Application.Models;

namespace eComm_Reporting_Application.Controllers
{
    public class MarMaxxReportsController : Controller
    {
        public IActionResult Index()
        {
            ReportPageDropdownModel marMaxxDropdownModel = new ReportPageDropdownModel();
            List<string> folder_list = new List<string>();
            
            folder_list.Add("Test folder 1");
            folder_list.Add("Test folder 2");
            folder_list.Add("Test folder 3");
            folder_list.Add("Test folder 4");

            marMaxxDropdownModel.folderList = folder_list;
            return View(marMaxxDropdownModel);
        }

        [HttpPost]
        public JsonResult GetReportNameValues(List<string> folderList)
        {
            try
            {
                return Json(folderList);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }

        }
    }

    
}
