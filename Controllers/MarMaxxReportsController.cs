using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace eComm_Reporting_Application.Controllers
{
    public class MarMaxxReportsController : Controller
    {
        private readonly IConfiguration configuration;
        private static string myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping.json");
        private static JObject jsonObject = JObject.Parse(myJsonString);

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
                var json_folders = jsonObject["folders"];

                foreach (string folder in folderPathList)
                {
                    var reportFolder = json_folders[folder];
                    var reports = reportFolder["reports"];
                    foreach (JProperty x in reports)
                    {
                        string report_name = x.Name;
                        reportNameList.Add(report_name);
                    }
                }
                
                return Json(reportNameList);
            }
            catch (Exception e)
            {
                return Json("Error retrieving report dropdown data: " + e);
            }
        }


        [HttpPost]
        public JsonResult GetMarMaxxTableData(string reportName)
        {
            try
            {
                ReportParameterModel tableParameters = getReportParameters(reportName);
                List<ReportTableModel> tableData = new List<ReportTableModel>();

                //Adding the static columns to the table (these will appear for every report)
                tableParameters.parameters.Insert(0, "Group_ID");
                tableParameters.parameters.Insert(0, "Group_Name");
                tableParameters.parameters.Insert(0, "Report_Name");
                tableParameters.parameters.Insert(0, "Subscription_Name");
                tableParameters.parameters.Insert(0, "Subscription_ID");

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Report_Name='" + reportName + "'";

                SqlCommand getTableData = new SqlCommand(queryString, connection);
                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = getTableData.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportTableModel tableRow = new ReportTableModel();
                            tableRow.subscriptionID = reader.GetInt32(0);
                            tableRow.subscriptionName = reader.GetString(1);
                            tableRow.reportName = reader.GetString(2);
                            tableRow.groupName = reader.GetString(3);
                            tableRow.groupID = reader.GetString(4);
                            tableData.Add(tableRow);
                        }
                    }
                    connection.Close();
                }

                return Json(new { tableParams = tableParameters.parameters, rowData = tableData });

            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }


        [HttpPost]
        public JsonResult GetMarMaxxReportParameters(string reportName)
        {
            var myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping.json");
            var jsonObject = JObject.Parse(myJsonString);
            

            //selecting folders object from json
            var folders = jsonObject["folders"];
            foreach(JProperty x in folders)
            {
                //grabs folder name
                string name = x.Name;
                //grabs the json associated with the folder name
                JToken value = x.Value;
            }

            var reportFolder = folders["Buyer Reports"];
            var reports = reportFolder["reports"];

            var testReport = reports["BCF Mix Master"];
            foreach (JProperty x in testReport)
            {
                string name = x.Name;
                JToken value = x.Value;
            }



            return Json(reportName);
        }

       public ReportPageDropdownModel getFoldersForDropdown()
        {
            ReportPageDropdownModel dropdownModel = new ReportPageDropdownModel();
            List<ReportFolderModel> folders = new List<ReportFolderModel>();
            try
            {
                //selecting folders object from json
                var json_folders = jsonObject["folders"];
                foreach (JProperty x in json_folders)
                {
                    ReportFolderModel folder = new ReportFolderModel();
                    folder.folderName = x.Name;
                    //folder.folderPath currently not used
                    folders.Add(folder);
                }
                dropdownModel.folders = folders;
            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }
            return dropdownModel;
        }

        public ReportParameterModel getReportParameters(string reportName)
        {
            List<ReportParameterModel> reportParameterModelList = new List<ReportParameterModel>();
            ReportParameterModel filteredReportParameters = new ReportParameterModel();
            try
            {
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
                    "SELECT * FROM #TempReportParameterTable WHERE Name IN ('" + reportName + "');";

                SqlCommand getReportParams = new SqlCommand(queryString, connection);
                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = getReportParams.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReportParameterModel reportParams = new ReportParameterModel();

                            //There is other data returned by this query that can be used - such as folder, fullpath, and hidden parameters.
                            //Currently only using report name and parameters
                            var report_name = reader.GetString(1);
                            if (reader.IsDBNull(3))
                            {
                                return filteredReportParameters; //Returning empty list of parameters if the report has no parameters
                            }
                            else
                            {
                                var report_params_string = reader.GetString(3);
                                List<string> report_params_list = new List<string>(report_params_string.Split(", "));
                                reportParams.reportName = report_name;
                                reportParams.parameters = report_params_list;

                                reportParameterModelList.Add(reportParams);
                            }
                        }
                    }
                    connection.Close();
                }

                //Filtering duplicate parameter data
                List<string> parameters = new List<string>();

                foreach (ReportParameterModel paramModel in reportParameterModelList)
                {
                    foreach (string parameter in paramModel.parameters)
                    {
                        string trimmed_param = parameter.Trim(' ');
                        if (!parameters.Contains(trimmed_param))
                        {
                            parameters.Add(trimmed_param);
                        }
                    }
                }
                filteredReportParameters.reportName = reportParameterModelList[0].reportName;
                filteredReportParameters.parameters = parameters;
            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }
            return filteredReportParameters;
        }
    }  
}
