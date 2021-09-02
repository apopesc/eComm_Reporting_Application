using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace eComm_Reporting_Application.Controllers
{
    public class MarMaxxReportsController : Controller
    {
        private readonly IConfiguration configuration;
        public MarMaxxReportsController(IConfiguration config)
        {
            this.configuration = config;
        }

        public IActionResult Index()
        {
            ReportPageDropdownModel marMaxxDropdownModel = new ReportPageDropdownModel();
            List<string> folder_list = new List<string>();

            string connectionstring = configuration.GetConnectionString("ReportServer");
            SqlConnection connection = new SqlConnection(connectionstring);

            SqlCommand getFolderList = new SqlCommand("SELECT Right(Path, Len(Path)-1) Folders, Path FROM dbo.Catalog WHERE Path NOT LIKE '%SubReports%' " +
                "AND Path NOT LIKE '%Data Sources%' AND TYPE=1 AND ParentID IS NOT NULL;", connection);

            using (connection)
            {
                connection.Open();
                using SqlDataReader reader = getFolderList.ExecuteReader();
                while (reader.Read())
                {
                    var folder = reader.GetString(0);
                    folder_list.Add(folder);
                }
            }

            marMaxxDropdownModel.folderList = folder_list;
            return View(marMaxxDropdownModel);
        }

        [HttpPost]
        public JsonResult GetReportNameValues(List<string> folderList)
        {
            try
            {
                List<string> reportNameList = new List<string>();
                //sql query to find report names using the folderlist
                //--------------------------------------------------------------
                reportNameList.Add("Test report 1");
                reportNameList.Add("Test report 2");
                reportNameList.Add("Test report 3");
                reportNameList.Add("Test report 4");
                //---------------------------------------------------------------

                return Json(reportNameList);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }

        public IActionResult AddNewReportSub()
        {
            return View();
        }
    }

    
}
