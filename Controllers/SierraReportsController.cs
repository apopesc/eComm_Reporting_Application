using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace eComm_Reporting_Application.Controllers
{
    [Authorize]
    public class SierraReportsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<SierraReportsController> _logger;

        private static string myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping - Sierra.json");
        private static JObject jsonObject = JObject.Parse(myJsonString);

        public static List<ReportTableModel> tableData = new List<ReportTableModel>();
        public static ReportParameterModel reportParams = new ReportParameterModel();
        public static ReportModel selectedReport = new ReportModel();
        public static bool changedReport = false;

        public SierraReportsController(IConfiguration config, ILogger<SierraReportsController> logger)
        {
            this.configuration = config;
            _logger = logger;
        }

        public IActionResult Error(string errorMsg)
        {
            ErrorViewModel errorModel = new ErrorViewModel();
            errorModel.errorMessage = errorMsg;
            return View(errorModel);
        }

        public IActionResult Index()
        {
            bool isAuthenticated = isAuthenticatedUser();

            if (isAuthenticated == false)
            {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            ReportPageDropdownModel sierraDropdownModel = GetFoldersForDropdown();

            return View(sierraDropdownModel);
        }

        public IActionResult AddNewReportSub(string selectedReportName)
        {
            bool isAuthenticated = isAuthenticatedUser();

            if (isAuthenticated == false)
            {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            ReportPageDropdownModel folderModel = GetFoldersForDropdown();
            UserSubscriptionDropdownModel groupModel = GetGroups();

            AddNewReportSubDropdownModel addNewDropdownModel = new AddNewReportSubDropdownModel();

            if (selectedReportName == "null")
            {
                tableData = new List<ReportTableModel>();
                reportParams = new ReportParameterModel();
                selectedReport = new ReportModel();
            }
            else
            {
                addNewDropdownModel.selectedFolder = selectedReport.reportFolder;
                addNewDropdownModel.selectedReport = selectedReport.reportName;
            }

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
        public JsonResult GetSierraTableData(ReportModel reportData)
        {
            try
            {
                if (reportData.reportName == null || reportData.reportName == "")
                {
                    return Json("Error retrieving table data: Report Name is empty");
                }
                else if (reportData.reportFolder == null || reportData.reportFolder == "")
                {
                    return Json("Error retrieving table data: Report Folder is empty");
                }
                else
                {
                    reportParams = GetReportParameters(reportData);
                    tableData = new List<ReportTableModel>();
                    selectedReport = reportData;

                    //Adding the static columns to the table (these will appear for every report)
                    Parameter schedule = new Parameter();
                    schedule.name = "Schedule";
                    reportParams.parameters.Insert(0, schedule);
                    Parameter fileFormat = new Parameter();
                    fileFormat.name = "File_Format";
                    reportParams.parameters.Insert(0, fileFormat);
                    Parameter groupID = new Parameter();
                    groupID.name = "Group_ID";
                    reportParams.parameters.Insert(0, groupID);
                    Parameter groupName = new Parameter();
                    groupName.name = "Group_Name";
                    reportParams.parameters.Insert(0, groupName);
                    Parameter reportName = new Parameter();
                    reportName.name = "Report_Name";
                    reportParams.parameters.Insert(0, reportName);
                    Parameter subscriptionName = new Parameter();
                    subscriptionName.name = "Subscription_Name";
                    reportParams.parameters.Insert(0, subscriptionName);
                    Parameter subscriptionID = new Parameter();
                    subscriptionID.name = "Subscription_ID";
                    reportParams.parameters.Insert(0, subscriptionID);


                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "SELECT * FROM SierraReportSubscriptions WHERE Report_Name='" + reportData.reportName + "'";

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
                                tableRow.groupNames = reader.GetString(3);
                                tableRow.groupIDs = reader.GetString(4);
                                tableRow.fileFormat = reader.GetString(6);
                                tableRow.schedule = reader.GetString(7);

                                string reportParamsJson = reader.GetString(5);
                                Dictionary<string, string> dynamicReportParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(reportParamsJson);
                                tableRow.dynamicParams = dynamicReportParams;

                                tableData.Add(tableRow);
                            }
                        }
                        connection.Close();
                    }

                    return Json(new { tableParams = reportParams.parameters, rowData = tableData });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving Sierra table data:  " + e);
                return Json("Error retrieving Sierra table data: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetInitialTable()
        {
            try
            {
                if (reportParams.parameters != null)
                {
                    if (reportParams.parameters.Count == 0 || reportParams.parameters[0].name != "Subscription_ID")
                    {
                        Parameter schedule = new Parameter();
                        schedule.name = "Schedule";
                        reportParams.parameters.Insert(0, schedule);
                        Parameter fileFormat = new Parameter();
                        fileFormat.name = "File_Format";
                        reportParams.parameters.Insert(0, fileFormat);
                        Parameter groupID = new Parameter();
                        groupID.name = "Group_ID";
                        reportParams.parameters.Insert(0, groupID);
                        Parameter groupName = new Parameter();
                        groupName.name = "Group_Name";
                        reportParams.parameters.Insert(0, groupName);
                        Parameter reportName = new Parameter();
                        reportName.name = "Report_Name";
                        reportParams.parameters.Insert(0, reportName);
                        Parameter subscriptionName = new Parameter();
                        subscriptionName.name = "Subscription_Name";
                        reportParams.parameters.Insert(0, subscriptionName);
                        Parameter subscriptionID = new Parameter();
                        subscriptionID.name = "Subscription_ID";
                        reportParams.parameters.Insert(0, subscriptionID);
                    }
                }

                return Json(new { tableParams = reportParams.parameters, rowData = tableData, report = selectedReport});
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving Sierra table data:  " + e);
                return Json("Error retrieving Sierra table data: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetSierraReportParameters(ReportModel reportData)
        {
            try
            {
                if (reportData != null && reportData.reportName != "" && reportData.reportFolder != "")
                {
                    reportParams = GetReportParameters(reportData);

                    if (reportData.reportName != selectedReport.reportName)
                    {
                        selectedReport = reportData;
                        changedReport = true;
                    }

                    string connectionstring = "";

                    //There are other data sources that need to be mapped here
                    if (reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                    }

                    for (int i = 0; i < reportParams.parameters.Count; i++)
                    {
                        if ((reportParams.parameters[i].type == "Dropdown" || reportParams.parameters[i].type == "Textbox" || reportParams.parameters[i].type == "MultiDropdown") && (reportParams.parameters[i].queryType == "Stored Procedure" || reportParams.parameters[i].queryType == "In Line"))
                        {
                            SqlConnection connection = new SqlConnection(connectionstring);
                            SqlCommand storedProcQuery = new SqlCommand(reportParams.parameters[i].query, connection);

                            if (reportParams.parameters[i].queryType == "Stored Procedure")
                            {
                                storedProcQuery.CommandType = CommandType.StoredProcedure;
                            }

                            using (connection)
                            {
                                List<string> dropdownValues = new List<string>();
                                List<string> dropdownLabels = new List<string>();

                                if (reportParams.parameters[i].name != "Department_No" && reportParams.parameters[i].name != "Class_Number" && reportParams.parameters[i].name != "Category") //these parameters take values from banner and each other to return data.
                                {
                                    connection.Open();
                                    using (SqlDataReader stored_proc_reader = storedProcQuery.ExecuteReader())
                                    {
                                        while (stored_proc_reader.Read())
                                        {
                                            var proc_data_length = stored_proc_reader.FieldCount;

                                            if (proc_data_length > 1)
                                            {
                                                for (int j = 0; j < proc_data_length; j++)
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
                                }

                                reportParams.parameters[i].values = dropdownValues;
                                reportParams.parameters[i].labels = dropdownLabels;
                            }
                        }
                    }

                    return Json(reportParams);
                }
                else
                {
                    return Json("Error retrieving report parameters: Report Name or Report Folder is empty");
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving report parameters:  " + e);
                return Json("Error retrieving report parameters: " + e);
            }

        }

        [HttpPost]
        public JsonResult SaveSierraReportSubscription(ReportTableModel reportSub)
        {
            try
            {
                if (reportSub.subscriptionName == "")
                {
                    return Json(new { message = "Error Saving Sierra Report Subscription: Subscription Name field is empty", result = "Error" });
                }
                else if (reportSub.reportName == "")
                {
                    return Json(new { message = "Error Saving Sierra Report Subscription: Report Name field is empty", result = "Error" });
                }
                else if (reportSub.groupIDs == "" || reportSub.groupNames == "")
                {
                    return Json(new { message = "Error Saving Sierra Report Subscription: Group field is empty", result = "Error" });
                }
                else
                {
                    int subscriptionID = 0;

                    string paramJson = JsonConvert.SerializeObject(reportSub.dynamicParams);
                    //Add query here to store in database, store group ID in their respective columns, and paramJson in the last column

                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);


                    //Checking if the group has at least one email tied to it, if not, return error.

                    string userGroupQueryString = "SELECT COUNT(*) FROM UserSubscriptions WHERE Group_ID IN('";
                    List<string> groupIDList = new List<string>(reportSub.groupIDs.Split(","));

                    for (int i = 0; i < groupIDList.Count; i++)
                    {

                        if (i < groupIDList.Count - 1)
                        {
                            userGroupQueryString = userGroupQueryString + groupIDList[i] + "','";
                        }
                        else
                        {
                            userGroupQueryString = userGroupQueryString + groupIDList[i] + "');";
                        }

                    }
                    SqlCommand usersInGroupQuery = new SqlCommand(userGroupQueryString, connection);

                    int usersInGroup = 0;

                    connection.Open();
                    using (SqlDataReader reader = usersInGroupQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var temp_userCount = reader.GetInt32(0);
                            usersInGroup = temp_userCount;
                        }
                    }
                    connection.Close();

                    if (usersInGroup < groupIDList.Count)
                    {
                        return Json(new { message = "One or more groups that have been selected have no users tied to them. You can add a user to a group on the User Subscriptions Groups screen.", result = "Error" });
                    }
                    else
                    {
                        string addUserQueryString = "INSERT INTO SierraReportSubscriptions (Subscription_Name, Report_Name, Group_Name, Group_ID, Report_Params, File_Format, Schedule) " +
                        "VALUES ('" + reportSub.subscriptionName + "', '" + reportSub.reportName + "', '" + reportSub.groupNames + "', '" + reportSub.groupIDs + "', '" + paramJson + "', '" + reportSub.fileFormat + "', '" + reportSub.schedule + "');";

                        SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);

                        string getSubIDString = "SELECT TOP 1* FROM SierraReportSubscriptions ORDER BY Subscription_ID Desc;"; //Getting the ID by getting the most recently added row
                        SqlCommand getSubID = new SqlCommand(getSubIDString, connection);

                        using (connection)
                        {
                            connection.Open();
                            using SqlDataReader reader = addUserQuery.ExecuteReader();
                            connection.Close();

                            connection.Open();
                            using (SqlDataReader reader_ID = getSubID.ExecuteReader())
                            {
                                while (reader_ID.Read())
                                {
                                    var id = reader_ID.GetInt32(0);
                                    subscriptionID = id;
                                }
                            }
                            connection.Close();
                        }


                        ReportTableModel newEntry = new ReportTableModel();
                        newEntry.subscriptionID = subscriptionID;
                        newEntry.subscriptionName = reportSub.subscriptionName;
                        newEntry.reportName = reportSub.reportName;
                        newEntry.groupNames = reportSub.groupNames;
                        newEntry.groupIDs = reportSub.groupIDs;
                        newEntry.fileFormat = reportSub.fileFormat;
                        newEntry.schedule = reportSub.schedule;
                        newEntry.dynamicParams = reportSub.dynamicParams;

                        if (changedReport == true)
                        {
                            tableData = new List<ReportTableModel>();
                            changedReport = false;
                        }

                        tableData.Insert(0, newEntry); //Adding new subscription to start of table

                        return Json(new { message = "Success saving subscription: ", result = "Redirect", url = Url.Action("Index", "SierraReports") });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving Marmaxx Report Subscription:  " + e);
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        public JsonResult DeleteSierraReportSubscription(int ID)
        {
            try
            {
                if (ID == 0)
                {
                    return Json(new { success = false, message = "Error Deleting Subscription: ID is empty" });
                }
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "DELETE FROM SierraReportSubscriptions WHERE Subscription_ID=" + ID;

                    SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                    using (connection)
                    {
                        connection.Open();
                        SqlDataReader reader = deleteUserQuery.ExecuteReader();
                        connection.Close();
                    }

                    tableData.RemoveAll(x => x.subscriptionID == ID);

                    return Json(new { success = true, message = "Success Deleting Subscription: " });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Deleting Subscription: " + e);
                return Json(new { success = false, message = "Error Deleting Subscription: " + e });
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
                _logger.LogError("Error retrieving folders: " + e);
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
                    if (param_name == "data_source")
                    {
                        JToken param_json = x.Value;
                        string data_source = param_json.Value<string>();
                        ReportParameters.dataSource = data_source;

                    }
                    else if (param_name == "parameters")
                    {
                        var json_params = report["parameters"];

                        //iterating through the parameters in the list
                        foreach (JProperty t in json_params)
                        {
                            Parameter reportParam = new Parameter();
                            reportParam.name = t.Name;
                            var param = json_params[reportParam.name];

                            foreach (JProperty z in param)
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
                                    case "default_val":
                                        reportParam.defaultVal = param_value;
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
                _logger.LogError("Error retrieving Sierra report parameters: " + e);
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

                string groupIDsQueryString = "SELECT * FROM Groups";

                SqlCommand groupIDsQuery = new SqlCommand(groupIDsQueryString, connection);

                using (connection)
                {
                    connection.Open();
                    using (SqlDataReader reader = groupIDsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var groupIDString = reader.GetString(0);
                            var groupString = reader.GetString(1);
                            groupsID_list.Add(groupIDString);
                            groups_list.Add(groupString);
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving groups: " + e);
            }

            UserSubscriptionDropdownModel groupModel = new UserSubscriptionDropdownModel()
            {
                groupsIDList = groupsID_list,
                groupsList = groups_list
            };

            return groupModel;
        }

       

        private bool isAuthenticatedUser()
        {
            string userName = User.Identity.Name;

            int userCount = 0;

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);
            string authUserQueryString = "SELECT COUNT(*) FROM [dbo].[User] WHERE UserName='" + userName + "';";
            SqlCommand authUserQuery = new SqlCommand(authUserQueryString, connection);

            connection.Open();
            using (SqlDataReader reader = authUserQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    var temp_userCount = reader.GetInt32(0);
                    userCount = temp_userCount;
                }
            }
            connection.Close();

            if (userCount < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
