using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

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
            ReportPageDropdownModel marMaxxDropdownModel = GetFoldersForDropdown();
            
            return View(marMaxxDropdownModel);
        }

        public IActionResult AddNewReportSub()
        {
            ReportPageDropdownModel folderModel = GetFoldersForDropdown();
            UserSubscriptionDropdownModel groupModel = GetGroups();

            AddNewReportSubDropdownModel addNewDropdownModel = new AddNewReportSubDropdownModel();
            addNewDropdownModel.folders = folderModel.folders;
            addNewDropdownModel.groupIDs = groupModel.groupsIDList;
            addNewDropdownModel.groupNames = groupModel.groupsList;

            return View(addNewDropdownModel);
        }

        [HttpPost]
        public JsonResult GetReportNameValues(List<string> folderPathList)
        {
            try
            {
                List<ReportModel> reportNameList = new List<ReportModel>();
                var json_folders = jsonObject["folders"];

                foreach (string folder in folderPathList)
                {
                    var reportFolder = json_folders[folder];
                    var reports = reportFolder["reports"];
                    foreach (JProperty x in reports)
                    {
                        ReportModel report = new ReportModel();
                        report.reportName = x.Name;
                        report.reportFolder = folder;
                        reportNameList.Add(report);
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
        public JsonResult GetMarMaxxTableData(ReportModel reportData)
        {
            try
            {
                ReportParameterModel tableParameters = GetReportParameters(reportData);
                List<ReportTableModel> tableData = new List<ReportTableModel>();

                //Adding the static columns to the table (these will appear for every report)
                Parameter groupID = new Parameter();
                groupID.name = "Group_ID";
                tableParameters.parameters.Insert(0, groupID);
                Parameter groupName = new Parameter();
                groupName.name = "Group_Name";
                tableParameters.parameters.Insert(0, groupName);
                Parameter reportName = new Parameter();
                reportName.name = "Report_Name";
                tableParameters.parameters.Insert(0, reportName);
                Parameter subscriptionName = new Parameter();
                subscriptionName.name = "Subscription_Name";
                tableParameters.parameters.Insert(0, subscriptionName);
                Parameter subscriptionID = new Parameter();
                subscriptionID.name = "Subscription_ID";
                tableParameters.parameters.Insert(0, subscriptionID);

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Report_Name='" + reportData.reportName + "'";

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

                            string reportParamsJson = reader.GetString(5);
                            Dictionary<string, string> reportParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(reportParamsJson);
                            tableRow.dynamicParams = reportParams;

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
        public JsonResult GetMarMaxxReportParameters(ReportModel reportData)
        {
            //ReportDataSource = Netsuite_ODS
            //eCom_ReportDB = eCom_ReportDB
            try
            {
                ReportParameterModel reportParams = GetReportParameters(reportData);
                string connectionstring = "";

                //There are other data sources that need to be mapped here
                if (reportParams.dataSource == "ReportDataSource")
                {
                    connectionstring = configuration.GetConnectionString("NetSuite_DB");
                }
                else if (reportParams.dataSource == "eCom_ReportDB")
                {
                    connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                }

                for (int i = 0; i < reportParams.parameters.Count; i++)
                {
                    if ( (reportParams.parameters[i].type == "Dropdown" || reportParams.parameters[i].type == "Textbox") && reportParams.parameters[i].queryType == "Stored Procedure")
                    {
                        SqlConnection connection = new SqlConnection(connectionstring);
                        SqlCommand storedProcQuery = new SqlCommand(reportParams.parameters[i].query, connection);
                        using (connection)
                        {
                            List<string> dropdownValues = new List<string>();
                            List<string> dropdownLabels = new List<string>();

                            connection.Open();
                            using (SqlDataReader stored_proc_reader = storedProcQuery.ExecuteReader())
                            {
                                while (stored_proc_reader.Read())
                                {
                                    var proc_data_length = stored_proc_reader.FieldCount;

                                    if (proc_data_length > 1)
                                    {
                                        for(int j = 0; j < proc_data_length; j++)
                                        {
                                            var proc_data_name = stored_proc_reader.GetName(j);
                                            if (proc_data_name == reportParams.parameters[i].values[0])
                                            {
                                                var proc_val = stored_proc_reader.GetValue(j);

                                                string dropdownVal = proc_val.ToString();
                                                dropdownValues.Add(dropdownVal);
                                            }

                                            if (proc_data_name == reportParams.parameters[i].labels[0])
                                            {
                                                var proc_label = stored_proc_reader.GetValue(j);

                                                string dropdownLab = proc_label.ToString();
                                                dropdownLabels.Add(dropdownLab);
                                            }
                                        }
                                    }
                                    else //if only one column is returned from the stored procedure, put in both labels and values
                                    {
                                        var proc_val = stored_proc_reader.GetValue(0);

                                        string dropdownEntry = proc_val.ToString();
                                        dropdownValues.Add(dropdownEntry);
                                        dropdownLabels.Add(dropdownEntry);
                                    }
                                }
                            }
                            connection.Close();

                            reportParams.parameters[i].values = dropdownValues;
                            reportParams.parameters[i].labels = dropdownLabels; 
                        }
                    }
                    else if ( (reportParams.parameters[i].type == "Dropdown" || reportParams.parameters[i].type == "Textbox") && reportParams.parameters[i].queryType == "In Line")
                    {
                        //Need to test code for inline queries
                    }
                }

                return Json(reportParams);
            }
            catch (Exception e)
            {
                return Json("Error retrieving report parameters: " + e);
            }

        }

        [HttpPost]
        public JsonResult SaveMarmaxxReportSubscription(ReportTableModel savedReportSub)
        {
            try
            {
                string paramJson = JsonConvert.SerializeObject(savedReportSub.dynamicParams);
                //Add query here to store in database, store group ID in their respective columns, and paramJson in the last column

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                string addUserQueryString = "INSERT INTO MarMaxxReportSubscriptions (Subscription_Name, Report_Name, Group_Name, Group_ID, Report_Params) " +
                    "VALUES ('" + savedReportSub.subscriptionName + "', '" + savedReportSub.reportName + "', '" + savedReportSub.groupName + "', '" + savedReportSub.groupID + "', '" + paramJson + "');";

                SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);

                using (connection)
                {
                    connection.Open();
                    using SqlDataReader reader = addUserQuery.ExecuteReader();
                    connection.Close();
                }

                return Json("Success saving report subscription: " + savedReportSub.subscriptionName);
            }
            catch (Exception e)
            {
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        public ReportPageDropdownModel GetFoldersForDropdown()
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

        public ReportParameterModel GetReportParameters(ReportModel reportData)
        {

            ReportParameterModel ReportParameters = new ReportParameterModel();
            try
            {
                var json_folders = jsonObject["folders"];
                var reportFolder = json_folders[reportData.reportFolder];
                var json_reports = reportFolder["reports"];
                var report = json_reports[reportData.reportName];

                ReportParameters.reportName = reportData.reportName;
                List<Parameter> parameters = new List<Parameter>();

                foreach (JProperty x in report)
                {
                    string param_name = x.Name;
                    if(param_name == "data_source")
                    {
                        JToken param_json = x.Value;
                        string data_source = param_json.Value<string>();
                        ReportParameters.dataSource = data_source;

                    } 
                    else if(param_name == "parameters")
                    {
                        var json_params = report["parameters"];

                        //iterating through the parameters in the list
                        foreach(JProperty t in json_params)
                        {
                            Parameter reportParam = new Parameter();
                            reportParam.name = t.Name;
                            var param = json_params[reportParam.name];
                            
                            foreach(JProperty z in param)
                            {
                                var name = z.Name;
                                JToken param_json = z.Value;
                                string param_value = param_json.Value<string>();

                                switch (name)
                                {
                                    case "type":
                                        reportParam.type = param_value;
                                        break;
                                    case "query_type":
                                        reportParam.queryType = param_value;
                                        break;
                                    case "query":
                                        reportParam.query = param_value;
                                        break;
                                    case "value":
                                        string[] val_array = param_value.Split(",");
                                        List<string> val_list = new List<string>(val_array);
                                        reportParam.values = val_list;
                                        break;
                                    case "label":
                                        string[] lab_array = param_value.Split(",");
                                        List<string> lab_list = new List<string>(lab_array);
                                        reportParam.labels = lab_list;
                                        break;
                                }
                            }
                            parameters.Add(reportParam);
                        } 
                    }
                }
                ReportParameters.parameters = parameters;
            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }
            return ReportParameters;
        }

        private UserSubscriptionDropdownModel GetGroups()
        {
            List<string> groups_list = new List<string>();
            List<string> groupsID_list = new List<string>();

            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string groupsQueryString = "SELECT DISTINCT User_Group FROM UserSubscriptionFilters WHERE User_Group IS NOT NULL";
                string groupIDsQueryString = "SELECT DISTINCT Group_ID FROM UserSubscriptionFilters WHERE Group_ID IS NOT NULL";

                SqlCommand groupsQuery = new SqlCommand(groupsQueryString, connection);
                SqlCommand groupIDsQuery = new SqlCommand(groupIDsQueryString, connection);

                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = groupsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var groupString = reader.GetString(0);
                            groups_list.Add(groupString);
                        }
                    }
                    using (SqlDataReader reader = groupIDsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var groupIDString = reader.GetString(0);
                            groupsID_list.Add(groupIDString);
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }

            UserSubscriptionDropdownModel groupModel = new UserSubscriptionDropdownModel()
            {
                groupsIDList = groupsID_list,
                groupsList = groups_list
            };

            return groupModel;
        }
    }
}
