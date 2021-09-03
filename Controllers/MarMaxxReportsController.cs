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
            ReportPageDropdownModel marMaxxDropdownModel = getFoldersForDropdown();
            
            return View(marMaxxDropdownModel);
        }

        public ReportPageDropdownModel getFoldersForDropdown()
        {
            ReportPageDropdownModel dropdownModel = new ReportPageDropdownModel();
            List<ReportFolderModel> folders = new List<ReportFolderModel>();
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportServer");
                SqlConnection connection = new SqlConnection(connectionstring);

                SqlCommand getFolderList = new SqlCommand("SELECT Right(Path, Len(Path)-1) Folders, Path FROM dbo.Catalog WHERE Path NOT LIKE '%SubReports%' " +
                    "AND Path NOT LIKE '%Data Sources%' AND TYPE=1 AND ParentID IS NOT NULL;", connection);

                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = getFolderList.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportFolderModel folder = new ReportFolderModel();
                            folder.folderName = reader.GetString(0);
                            folder.folderPath = reader.GetString(1);
                            folders.Add(folder);
                        }
                    }
                    connection.Close();
                }
                dropdownModel.folders = folders;
            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }
            return dropdownModel;
        }

        [HttpPost]
        public JsonResult GetReportNameValues(List<string> folderPathList)
        {
            try
            {
                string folderListString = String.Join("', '", folderPathList.ToArray());
                folderListString = "'" + folderListString + "'";
                List<string> reportNameList = new List<string>();

                string connectionstring = configuration.GetConnectionString("ReportServer");
                SqlConnection connection = new SqlConnection(connectionstring);

                SqlCommand getFolderList = new SqlCommand("DROP TABLE IF EXISTS #TempReportPathTable SELECT Name, LEFT(Path, Len(Path)-Len(Name)-1) Path, Path as FullPath " +
                    "INTO #TempReportPathTable FROM dbo.Catalog WHERE Path NOT LIKE '%SubReports%' AND Path NOT LIKE '%Data Sources%' AND TYPE = 2 " +
                    "SELECT * FROM #TempReportPathTable WHERE Path IN ("+folderListString+");", connection);
                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = getFolderList.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var reportName = reader.GetString(0);
                            reportNameList.Add(reportName);
                        }
                    }
                    connection.Close();
                }

                return Json(reportNameList);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }

        public IActionResult AddNewReportSub()
        {
            ReportPageDropdownModel addNewDropdownModel = getFoldersForDropdown();

            return View(addNewDropdownModel);
        }


        [HttpPost]
        public JsonResult GetMarMaxxTableData (ReportPageDropdownModel filterData)
        {
            try
            {
                return Json("Controller function reached");
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }
    }

    
}
