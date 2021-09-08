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

        public IActionResult AddNewReportSub()
        {
            ReportPageDropdownModel addNewDropdownModel = getFoldersForDropdown();

            return View(addNewDropdownModel);
        }

        [HttpPost]
        public JsonResult GetReportNameValues(List<string> folderPathList)
        {
            try
            {
                List<string> reportNameList = new List<string>();

                string folderListString = String.Join("', '", folderPathList.ToArray());
                folderListString = "'" + folderListString + "'";

                string connectionstring = configuration.GetConnectionString("ReportServer");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "DROP TABLE IF EXISTS #TempReportPathTable SELECT Name, LEFT(Path, Len(Path)-Len(Name)-1) Path, Path as FullPath " +
                    "INTO #TempReportPathTable FROM dbo.Catalog WHERE Path NOT LIKE '%SubReports%' AND Path NOT LIKE '%Data Sources%' AND TYPE = 2 " +
                    "SELECT * FROM #TempReportPathTable WHERE Path IN (" + folderListString + ");";

                SqlCommand getFolderList = new SqlCommand(queryString, connection);
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
                return Json("Error retrieving report dropdown data: " + e);
            }
        }


        [HttpPost]
        public JsonResult GetMarMaxxTableData(ReportPageDropdownModel filterData)
        {
            try
            {
                List<ReportParameterModel> tableParameters = new List<ReportParameterModel>();

                string connectionstring = configuration.GetConnectionString("ReportServer");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "DROP TABLE IF EXISTS #Parameters DROP TABLE IF EXISTS #TempReportParameterTable " +
                    "SELECT Folder,Report_Name,Full_Path,Parameter = Paravalue.value('Name[1]', 'VARCHAR(250)'),Type = Paravalue.value('Type[1]', 'VARCHAR(250)')," +
                    "Nullable = Paravalue.value('Nullable[1]', 'VARCHAR(250)'),AllowBlank = Paravalue.value('AllowBlank[1]', 'VARCHAR(250)'),MultiValue = Paravalue.value('MultiValue[1]', 'VARCHAR(250)')," +
                    "UsedInQuery = Paravalue.value('UsedInQuery[1]', 'VARCHAR(250)'),Prompt = Paravalue.value('Prompt[1]', 'VARCHAR(250)'),DynamicPrompt = Paravalue.value('DynamicPrompt[1]', 'VARCHAR(250)')," +
                    "PromptUser = Paravalue.value('PromptUser[1]', 'VARCHAR(250)'),State = Paravalue.value('State[1]', 'VARCHAR(250)') " +
                    "INTO #Parameters " +
                    "FROM (SELECT LEFT(Path, Len(Path)-Len(Name)-1) AS Folder,Name AS Report_Name,Path as Full_Path,CONVERT(XML,C.Parameter) AS ParameterXML FROM ReportServer.dbo.Catalog C WHERE C.Content IS NOT NULL AND C.Type = 2) a " +
                    "CROSS APPLY ParameterXML.nodes('//Parameters/Parameter') p ( Paravalue ) " +
                    "SELECT LEFT(Path, Len(Path)-Len(Name)-1) AS Folder,Name,Path as FullPath," +
                    "STUFF((SELECT ', '+ISNULL(Parameter,'') FROM #Parameters P WHERE C.Name = P.Report_Name AND PromptUser = 'True' AND Parameter NOT LIKE '%hidden%' AND Parameter NOT IN ('NoMonths','Job_Type') FOR XML PATH ('')),1, 1, '') AS [Parameters]," +
                    "STUFF((SELECT ', '+ISNULL(Parameter,'') FROM #Parameters P WHERE C.Name = P.Report_Name FOR XML PATH ('')),1, 1, '') AS Parameters_WithHidden " +
                    "INTO #TempReportParameterTable " +
                    "FROM ReportServer.dbo.Catalog C WITH(NOLOCK) WHERE TYPE = 2 AND Left(Path, Len(Path)-Len(Name)-1) NOT IN ('/DevTest Reports','/PreProdTestReports') ORDER BY 1,2 " +
                    "SELECT * FROM #TempReportParameterTable WHERE Name IN ('" + filterData.reportName + "');";

                SqlCommand getFolderList = new SqlCommand(queryString, connection);
                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = getFolderList.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportParameterModel reportParams = new ReportParameterModel();

                            //There is other data returned by this query that can be used - such as folder, fullpath, and hidden parameters.
                            //Currently only using report name and parameters
                            var report_name = reader.GetString(1);
                            if (reader.IsDBNull(3))
                            {
                                return Json(report_name + " has no parameters, please select a report with parameters");
                            }
                            else
                            {
                                var report_params_string = reader.GetString(3);
                                List<string> report_params_list = new List<string>(report_params_string.Split(", "));
                                reportParams.reportName = report_name;
                                reportParams.parameters = report_params_list;

                                tableParameters.Add(reportParams);
                            }
                        }
                    }
                    connection.Close();
                }
                return Json(tableParameters);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }

        public ReportPageDropdownModel getFoldersForDropdown()
        {
            ReportPageDropdownModel dropdownModel = new ReportPageDropdownModel();
            List<ReportFolderModel> folders = new List<ReportFolderModel>();
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportServer");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "SELECT Right(Path, Len(Path)-1) Folders, Path FROM dbo.Catalog WHERE Path NOT LIKE '%SubReports%' " +
                    "AND Path NOT LIKE '%Data Sources%' AND TYPE=1 AND ParentID IS NOT NULL;";

                SqlCommand getFolderList = new SqlCommand(queryString, connection);
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

    }  
}
