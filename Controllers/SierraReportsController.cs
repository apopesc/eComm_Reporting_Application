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
using System.Text.RegularExpressions;
using eComm_Reporting_Application.Extensions;

namespace eComm_Reporting_Application.Controllers
{
    [Authorize]
    public class SierraReportsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<SierraReportsController> _logger;

        private static string myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping - Sierra.json");
        private static JObject jsonObject = JObject.Parse(myJsonString);

        //public static List<ReportTableModel> tableData = new List<ReportTableModel>();
        //public static ReportParameterModel reportParams = new ReportParameterModel();
        //public static ReportModel selectedReport = new ReportModel();
        //public static bool changedReport = false;

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
                List<ReportTableModel> tableData = new List<ReportTableModel>();
                HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", tableData);

                ReportParameterModel reportParams = new ReportParameterModel();
                HttpContext.Session.SetObjectAsJson<ReportParameterModel>("sierraReportParams", reportParams);

                ReportModel selectedReport = new ReportModel();
                HttpContext.Session.SetObjectAsJson<ReportModel>("selectedSierraReport", selectedReport);
            }
            else
            {
                ReportModel selectedReport = HttpContext.Session.GetObjectFromJson<ReportModel>("selectedSierraReport");
                addNewDropdownModel.selectedFolder = selectedReport.reportFolder;
                addNewDropdownModel.selectedReport = selectedReport.reportName;
            }

            addNewDropdownModel.folders = folderModel.folders;
            addNewDropdownModel.groupIDs = groupModel.groupsIDList;
            addNewDropdownModel.groupNames = groupModel.groupsList;

            return View(addNewDropdownModel);
        }

        public IActionResult EditReportSub(int ID, bool copy)
        {
            bool isAuthenticated = isAuthenticatedUser();

            if (isAuthenticated == false)
            {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            EditReportSubscriptionModel reportSubModel = new EditReportSubscriptionModel();

            reportSubModel.isCopy = copy;

            string queryString = "SELECT * FROM SierraReportSubscriptions WHERE Subscription_ID=@ID;";

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlCommand getSubscriptionData = new SqlCommand(queryString, connection);

            getSubscriptionData.Parameters.AddWithValue("@ID", ID);

            using (connection)
            {
                connection.Open();
                using (SqlDataReader reader = getSubscriptionData.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reportSubModel.subscriptionID = reader.GetInt32(0);
                        reportSubModel.subscriptionName = reader.GetString(1);
                        reportSubModel.reportName = reader.GetString(2);
                        reportSubModel.selectedGroupNames = reader.GetString(3);
                        reportSubModel.selectedGroupIDs = reader.GetString(4);
                        reportSubModel.selectedFileFormat = reader.GetString(6);
                        reportSubModel.selectedSchedule = reader.GetString(7);

                        string report_params_json = reader.GetString(5);
                        Dictionary<string, string> dynamicReportParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(report_params_json);
                        reportSubModel.dynamicParams = dynamicReportParams;
                    }
                }
                connection.Close();
            }

            UserSubscriptionDropdownModel dropdownData = GetGroups();
            reportSubModel.groupNames = dropdownData.groupsList;
            reportSubModel.groupIDs = dropdownData.groupsIDList;

            var json_folders = jsonObject["folders"];
            foreach (JProperty x in json_folders)
            {
                string folderName = x.Name;

                var reportFolder = json_folders[folderName];
                var json_reports = reportFolder["reports"];
                foreach (JProperty y in json_reports)
                {
                    string temp_report_name = y.Name;
                    if (temp_report_name == reportSubModel.reportName)
                    {
                        reportSubModel.folderName = folderName;
                    }
                }
            }

            return View(reportSubModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public JsonResult GetSierraTableData([Bind("reportName,reportFolder")] ReportModel reportData)
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
                    ReportParameterModel sierraReportParams = GetReportParameters(reportData);

                    List<ReportTableModel> newSierraTableData = new List<ReportTableModel>();
                    HttpContext.Session.SetObjectAsJson<ReportModel>("selectedSierraReport", reportData);

                    ReportParameterModel reportSpecificParams = new ReportParameterModel();
                    reportSpecificParams.parameters = new List<Parameter>();

                    foreach (Parameter r_param in sierraReportParams.parameters)
                    {
                        reportSpecificParams.parameters.Add(r_param);
                    }

                    //Adding the static columns to the table (these will appear for every report)
                    Parameter schedule = new Parameter();
                    schedule.name = "Schedule";
                    sierraReportParams.parameters.Insert(0, schedule);
                    Parameter fileFormat = new Parameter();
                    fileFormat.name = "File_Format";
                    sierraReportParams.parameters.Insert(0, fileFormat);
                    Parameter groupID = new Parameter();
                    groupID.name = "Group_ID";
                    sierraReportParams.parameters.Insert(0, groupID);
                    Parameter groupName = new Parameter();
                    groupName.name = "Group_Name";
                    sierraReportParams.parameters.Insert(0, groupName);
                    Parameter reportName = new Parameter();
                    reportName.name = "Report_Name";
                    sierraReportParams.parameters.Insert(0, reportName);
                    Parameter subscriptionName = new Parameter();
                    subscriptionName.name = "Subscription_Name";
                    sierraReportParams.parameters.Insert(0, subscriptionName);
                    Parameter subscriptionID = new Parameter();
                    subscriptionID.name = "Subscription_ID";
                    sierraReportParams.parameters.Insert(0, subscriptionID);


                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "SELECT * FROM SierraReportSubscriptions WHERE Report_Name=@reportName";

                    SqlCommand getTableData = new SqlCommand(queryString, connection);
                    getTableData.Parameters.AddWithValue("@reportName", reportData.reportName);

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

                                for (int i = 0; i < reportSpecificParams.parameters.Count; i++)
                                {
                                    if (!(dynamicReportParams.ContainsKey(reportSpecificParams.parameters[i].name)))
                                    {
                                        dynamicReportParams[reportSpecificParams.parameters[i].name] = "";
                                    }
                                }

                                tableRow.dynamicParams = dynamicReportParams;

                                newSierraTableData.Add(tableRow);
                            }
                        }
                        connection.Close();
                    }


                    HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", newSierraTableData);
                    HttpContext.Session.SetObjectAsJson<ReportParameterModel>("sierraReportParams", sierraReportParams);

                    return Json(new { tableParams = sierraReportParams.parameters, rowData = newSierraTableData });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving Sierra table data:  " + e);
                return Json("Error retrieving Sierra table data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInitialTable()
        {
            try
            {
                List<ReportTableModel> tableData = HttpContext.Session.GetObjectFromJson<List<ReportTableModel>>("sierraSubTableData");
                ReportParameterModel reportParams = HttpContext.Session.GetObjectFromJson<ReportParameterModel>("sierraReportParams");
                ReportModel selectedReport = HttpContext.Session.GetObjectFromJson<ReportModel>("selectedSierraReport");

                if (tableData == null)
                {
                    tableData = new List<ReportTableModel>();
                }

                if (reportParams == null)
                {
                    reportParams = new ReportParameterModel();
                }

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
        [ValidateAntiForgeryToken]
        public JsonResult GetSierraReportParameters([Bind("reportName,reportFolder")] ReportModel reportData)
        {
            try
            {
                if (reportData != null && reportData.reportName != "" && reportData.reportFolder != "")
                {
                    ReportParameterModel sierraReportParams = GetReportParameters(reportData);
                    ReportModel selectedSierraReport = HttpContext.Session.GetObjectFromJson<ReportModel>("selectedSierraReport");

                    if (reportData.reportName != selectedSierraReport.reportName)
                    {
                        HttpContext.Session.SetObjectAsJson<ReportModel>("selectedSierraReport", reportData);
                        HttpContext.Session.SetObjectAsJson<bool>("changedSierraReport", true);
                    }

                    string connectionstring = "";

                    //There are other data sources that need to be mapped here
                    if (sierraReportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                    }

                    for (int i = 0; i < sierraReportParams.parameters.Count; i++)
                    {
                        if ((sierraReportParams.parameters[i].type == "Dropdown" || sierraReportParams.parameters[i].type == "Textbox" || sierraReportParams.parameters[i].type == "MultiDropdown") && (sierraReportParams.parameters[i].queryType == "Stored Procedure" || sierraReportParams.parameters[i].queryType == "In Line"))
                        {

                            SqlConnection connection = new SqlConnection(connectionstring);

                            string procQueryString = "";

                            if (sierraReportParams.parameters[i].queryType == "Stored Procedure")
                            {
                                procQueryString = "EXEC @storedProc";
                            }
                            else if (sierraReportParams.parameters[i].queryType == "In Line") //SQL Injection Risk in veracode
                            {
                                procQueryString = sierraReportParams.parameters[i].query;
                            }

                            SqlCommand storedProcQuery = new SqlCommand(procQueryString, connection);

                            if (sierraReportParams.parameters[i].queryType == "Stored Procedure")
                            {
                                storedProcQuery.Parameters.AddWithValue("@storedProc", sierraReportParams.parameters[i].query);
                            }

                            //storedProcQuery.CommandType = CommandType.StoredProcedure;

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
                                            for (int j = 0; j < proc_data_length; j++)
                                            {
                                                var proc_data_name = stored_proc_reader.GetName(j);
                                                if (proc_data_name == sierraReportParams.parameters[i].values[0])
                                                {
                                                    var proc_val = stored_proc_reader.GetValue(j);

                                                    string dropdownVal = proc_val.ToString();
                                                    dropdownValues.Add(dropdownVal);
                                                }

                                                if (proc_data_name == sierraReportParams.parameters[i].labels[0])
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


                                sierraReportParams.parameters[i].values = dropdownValues;
                                sierraReportParams.parameters[i].labels = dropdownLabels;
                            }

                        }
                    }

                    HttpContext.Session.SetObjectAsJson<ReportParameterModel>("sierraReportParams", sierraReportParams);
                    return Json(new { success = true, reportParams = sierraReportParams });
                }
                else
                {
                    return Json(new { success = false, message = "Error retrieving report parameters: Report Name or Report Folder is empty" });
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving report parameters:  " + e);
                return Json(new { success = false, message = "Error retrieving report parameters: " + e });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveSierraReportSubscription([Bind("subscriptionID,subscriptionName,reportName,groupNames,groupIDs,fileFormat,schedule,dynamicParams")] ReportTableModel reportSub)
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

                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);

                    List<string> groupIDList = new List<string>(reportSub.groupIDs.Split(","));
                    var groupIDParams = new string[groupIDList.Count];
                    SqlCommand usersInGroupQuery = new SqlCommand();

                    for (int i = 0; i < groupIDList.Count; i++)
                    {
                        groupIDParams[i] = string.Format("@GroupID{0}", i);
                        usersInGroupQuery.Parameters.AddWithValue(groupIDParams[i], groupIDList[i]);
                    }

                    //Checking if the group has at least one email tied to it, if not, return error.
                    usersInGroupQuery.CommandText = string.Format("SELECT COUNT(*) FROM UserSubscriptions WHERE Group_ID IN ({0})", string.Join(",", groupIDParams));
                    usersInGroupQuery.Connection = connection;

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
                        connection = new SqlConnection(connectionstring);

                        string addUserQueryString = "INSERT INTO SierraReportSubscriptions (Subscription_Name, Report_Name, Group_Name, Group_ID, Report_Params, File_Format, Schedule) " +
                        "VALUES (@subscriptionName, @reportName, @groupNames, @groupIDs, @paramJson, @fileFormat, @schedule);";

                        SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);
                        addUserQuery.Parameters.AddWithValue("@subscriptionName", reportSub.subscriptionName);
                        addUserQuery.Parameters.AddWithValue("@reportName", reportSub.reportName);
                        addUserQuery.Parameters.AddWithValue("@groupNames", reportSub.groupNames);
                        addUserQuery.Parameters.AddWithValue("@groupIDs", reportSub.groupIDs);
                        addUserQuery.Parameters.AddWithValue("@paramJson", paramJson);
                        addUserQuery.Parameters.AddWithValue("@fileFormat", reportSub.fileFormat);
                        addUserQuery.Parameters.AddWithValue("@schedule", reportSub.schedule);

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

                        bool changedReport = HttpContext.Session.GetObjectFromJson<bool>("changedSierraReport");

                        if (changedReport == true)
                        {
                            List<ReportTableModel> sierraTableData = new List<ReportTableModel>();
                            HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", sierraTableData);
                            HttpContext.Session.SetObjectAsJson<bool>("changedSierraReport", false);
                        }

                        List<ReportTableModel> currentTableData = HttpContext.Session.GetObjectFromJson<List<ReportTableModel>>("sierraSubTableData");

                        if (currentTableData == null)
                        {
                            currentTableData = new List<ReportTableModel>();
                        }

                        currentTableData.Insert(0, newEntry);
                        HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", currentTableData);

                        return Json(new { message = "Success saving subscription: ", result = "Redirect", url = Url.Action("Index", "SierraReports") });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving Sierra Report Subscription:  " + e);
                return Json("Error Saving Sierra Report Subscription: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveEditedSierraReportSubscription([Bind("subscriptionID,subscriptionName,reportName,groupNames,groupIDs,fileFormat,schedule,dynamicParams")] ReportTableModel reportSub)
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
                    string paramJson = JsonConvert.SerializeObject(reportSub.dynamicParams);
                    //Add query here to store in database, store group ID in their respective columns, and paramJson in the last column

                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    //Checking if the group has at least one email tied to it, if not, return error.

                    List<string> groupIDList = new List<string>(reportSub.groupIDs.Split(","));
                    var groupIDParams = new string[groupIDList.Count];
                    SqlCommand usersInGroupQuery = new SqlCommand();

                    for (int i = 0; i < groupIDList.Count; i++)
                    {
                        groupIDParams[i] = string.Format("@GroupID{0}", i);
                        usersInGroupQuery.Parameters.AddWithValue(groupIDParams[i], groupIDList[i]);
                    }

                    //Checking if the group has at least one email tied to it, if not, return error.
                    usersInGroupQuery.CommandText = string.Format("SELECT COUNT(*) FROM UserSubscriptions WHERE Group_ID IN ({0}) AND Is_Active = 'Y'", string.Join(",", groupIDParams));
                    usersInGroupQuery.Connection = connection;

                    int usersInGroup = 0;

                    using (connection)
                    {
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
                    }

                    if (usersInGroup < groupIDList.Count)
                    {
                        return Json(new { message = "One or more groups that have been selected have no ACTIVE users tied to them. You can add an ACTIVE user to a group on the User Subscriptions Groups screen.", result = "Error" });
                    }
                    else
                    {
                        connection = new SqlConnection(connectionstring);

                        string editUserQueryString = "UPDATE SierraReportSubscriptions SET Subscription_Name=@subscriptionName, Report_Name=@reportName, Group_Name=@groupNames, Group_ID=@groupIDs, Report_Params=@paramJson, File_Format=@fileFormat, Schedule=@schedule " +
                        "WHERE Subscription_ID=@subscriptionID;";

                        SqlCommand editUserQuery = new SqlCommand(editUserQueryString, connection);
                        editUserQuery.Parameters.AddWithValue("@subscriptionName", reportSub.subscriptionName);
                        editUserQuery.Parameters.AddWithValue("@reportName", reportSub.reportName);
                        editUserQuery.Parameters.AddWithValue("@groupNames", reportSub.groupNames);
                        editUserQuery.Parameters.AddWithValue("@groupIDs", reportSub.groupIDs);
                        editUserQuery.Parameters.AddWithValue("@paramJson", paramJson);
                        editUserQuery.Parameters.AddWithValue("@fileFormat", reportSub.fileFormat);
                        editUserQuery.Parameters.AddWithValue("@schedule", reportSub.schedule);
                        editUserQuery.Parameters.AddWithValue("@subscriptionID", reportSub.subscriptionID);

                        using (connection)
                        {
                            connection.Open();
                            using SqlDataReader reader = editUserQuery.ExecuteReader();
                            connection.Close();
                        }

                        List<ReportTableModel> currentTableData = HttpContext.Session.GetObjectFromJson<List<ReportTableModel>>("sierraSubTableData");
                        for (int i = 0; i < currentTableData.Count; i++)
                        {
                            if (currentTableData[i].subscriptionID == reportSub.subscriptionID)
                            {
                                currentTableData[i].subscriptionName = reportSub.subscriptionName;
                                currentTableData[i].reportName = reportSub.reportName;
                                currentTableData[i].groupNames = reportSub.groupNames;
                                currentTableData[i].groupIDs = reportSub.groupIDs;
                                currentTableData[i].fileFormat = reportSub.fileFormat;
                                currentTableData[i].schedule = reportSub.schedule;
                                currentTableData[i].dynamicParams = reportSub.dynamicParams;

                                break;
                            }

                        }
                        HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", currentTableData);

                        return Json(new { message = "Success editing subscription: ", result = "Redirect", url = Url.Action("Index", "SierraReports") });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving Sierra Report Subscription:  " + e);
                return Json("Error Saving Sierra Report Subscription: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                    string queryString = "DELETE FROM SierraReportSubscriptions WHERE Subscription_ID=@ID";

                    SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                    deleteUserQuery.Parameters.AddWithValue("@ID", ID);

                    using (connection)
                    {
                        connection.Open();
                        SqlDataReader reader = deleteUserQuery.ExecuteReader();
                        connection.Close();
                    }

                    List<ReportTableModel> currentTableData = HttpContext.Session.GetObjectFromJson<List<ReportTableModel>>("sierraSubTableData");
                    currentTableData.RemoveAll(x => x.subscriptionID == ID);
                    HttpContext.Session.SetObjectAsJson<List<ReportTableModel>>("sierraSubTableData", currentTableData);

                    return Json(new { success = true, message = "Success Deleting Subscription: " });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Deleting Subscription: " + e);
                return Json(new { success = false, message = "Error Deleting Subscription: " + e });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetClassData([FromBody][Bind("reportData,selectedDepartments")] DepartmentModel departmentModel)
        {
            try
            {
                if (departmentModel.selectedDepartments.Count == 0)
                {
                    return Json("Error Getting Class Data: Department field is empty");
                }
                else if (departmentModel.reportData == null || departmentModel.reportData.reportName == "" || departmentModel.reportData.reportFolder == "")
                {
                    return Json("Error Getting Class Data: Report Name or Report Folder is empty");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(departmentModel.reportData);
                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    Parameter classParameter = local_reportParams.parameters.Find(x => x.name == "Class_Number");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(classParameter.query, connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    string selectedDepartmentsString = string.Join(",", departmentModel.selectedDepartments.ToArray());
                    storedProcQuery.Parameters.AddWithValue("@Department_No", selectedDepartmentsString);

                    //Parameter populatedParam = getCascadingDropdownValues(classParameter, storedProcQuery, connection);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = classParameter;

                    using (connection)
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
                                        if (proc_data_name == classParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == classParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;

                    for (int i = 0; i < populatedParam.values.Count; i++)
                    {
                        int charLocation = populatedParam.values[i].IndexOf("~");
                        if (charLocation > -1)
                        {
                            string parsedClass = populatedParam.values[i].Substring(0, charLocation);
                            populatedParam.values[i] = parsedClass;
                        }
                    }

                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Class Data: " + e);
                return Json("Error Getting Class Data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetCategoryData([FromBody][Bind("reportData,selectedDepartments,selectedClasses")] ClassModel classModel)
        {
            try
            {
                if (classModel.selectedDepartments.Count == 0)
                {
                    return Json("Error Getting Category Data: Department field is empty");
                }
                else if (classModel.selectedClasses.Count == 0)
                {
                    return Json("Error Getting Category Data: Class field is empty");
                }
                else if (classModel.reportData == null || classModel.reportData.reportName == "" || classModel.reportData.reportFolder == "")
                {
                    return Json("Error Getting Category Data: Report Name or Report Folder is empty");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(classModel.reportData);
                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    Parameter categoryParameter = local_reportParams.parameters.Find(x => x.name == "Category");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(categoryParameter.query, connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    string selectedDepartmentsString = string.Join(",", classModel.selectedDepartments.ToArray());
                    storedProcQuery.Parameters.AddWithValue("@Department_No", selectedDepartmentsString);



                    for (int i = 0; i < classModel.selectedClasses.Count; i++)
                    {
                        classModel.selectedClasses[i] = classModel.selectedClasses[i] + "~";
                    }


                    string selectedClassesString = string.Join(",", classModel.selectedClasses.ToArray());
                    storedProcQuery.Parameters.AddWithValue("@Class_Number", selectedClassesString);
                    storedProcQuery.CommandTimeout = 350;

                    //Parameter populatedParam = getCascadingDropdownValues(categoryParameter, storedProcQuery, connection);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = categoryParameter;

                    using (connection)
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
                                        if (proc_data_name == categoryParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == categoryParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;


                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Category Data: " + e);
                return Json("Error Getting Category Data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetBrandData([FromBody][Bind("reportData,brandPattern")] BrandModel brandModel)
        {
            try
            {
                if (brandModel.brandPattern == "")
                {
                    return Json("Error Getting Brand Data: Brand_Pattern field is empty");
                }
                else if (brandModel.reportData == null || brandModel.reportData.reportName == "" || brandModel.reportData.reportFolder == "")
                {
                    return Json("Error Getting Brand Data: Report Name or Report Folder is empty");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(brandModel.reportData);

                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();
                    

                    Parameter brandParameter = local_reportParams.parameters.Find(x => x.name == "Brand");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(brandParameter.query, connection);
                    }
                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Brand_Pattern", brandModel.brandPattern);

                    //Parameter populatedParam = getCascadingDropdownValues(brandParameter, storedProcQuery, connection);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = brandParameter;

                    using (connection)
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
                                        if (proc_data_name == brandParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == brandParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;

                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Brand Data: " + e);
                return Json("Error Getting Brand Data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetVendorData([FromBody][Bind("reportData,vendorPattern")] VendorModel vendorModel)
        {
            try
            {
                if (vendorModel.vendorPattern == "")
                {
                    return Json("Error Getting Brand Data: Brand_Pattern field is empty");
                }
                else if (vendorModel.reportData == null || vendorModel.reportData.reportName == "" || vendorModel.reportData.reportFolder == "")
                {
                    return Json("Error Getting Brand Data: Report Name or Report Folder is empty");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(vendorModel.reportData);

                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    Parameter vendorParameter = local_reportParams.parameters.Find(x => x.name == "Vendor");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(vendorParameter.query, connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Vendor_Pattern", vendorModel.vendorPattern);

                    //Parameter populatedParam = getCascadingDropdownValues(vendorParameter, storedProcQuery, connection);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = vendorParameter;

                    using (connection)
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
                                        if (proc_data_name == vendorParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == vendorParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;

                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Vendor Data: " + e);
                return Json("Error Getting Vendor Data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetStoreGroupData([Bind("reportName,reportFolder")] ReportModel reportData, string channel)
        {
            try
            {
                if (channel == null || channel == "") 
                {
                    return Json("Error Getting Store Group Data: Channel is Empty.");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(reportData);

                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    Parameter storeGroupParameter = local_reportParams.parameters.Find(x => x.name == "StoreGroup");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(storeGroupParameter.query, connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Channel", channel);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = storeGroupParameter;

                    using (connection)
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
                                        if (proc_data_name == storeGroupParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == storeGroupParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;

                    if(populatedParam.values.Count < 1)
                    {
                        populatedParam.values.Add("Web");
                        populatedParam.labels.Add("Web");
                    }

                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Store Group Data: " + e);
                return Json("Error Getting Store Group Data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetLocationData([Bind("reportName,reportFolder")] ReportModel reportData, string channel)
        {
            try
            {
                if (channel == null || channel == "")
                {
                    return Json("Error Getting Store Group Data: Channel is Empty.");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(reportData);

                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    Parameter locationParameter = local_reportParams.parameters.Find(x => x.name == "Location");

                    if (local_reportParams.dataSource == "STP_CMS_DW")
                    {
                        connectionstring = configuration.GetConnectionString("STP_CMS_DW");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand(locationParameter.query, connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Channel", channel);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = locationParameter;

                    using (connection)
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
                                        if (proc_data_name == locationParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == locationParameter.labels[0])
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

                    populatedParam.values = dropdownValues;
                    populatedParam.labels = dropdownLabels;

                    if (populatedParam.values.Count < 1)
                    {
                        populatedParam.values.Add("Web");
                        populatedParam.labels.Add("No Stores");
                    }

                    return Json(populatedParam);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Location Data: " + e);
                return Json("Error Getting Location Data: " + e);
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
            string authUserQueryString = "SELECT COUNT(*) FROM [dbo].[User] WHERE UserName=@userName;";
            SqlCommand authUserQuery = new SqlCommand(authUserQueryString, connection);
            authUserQuery.Parameters.AddWithValue("@userName", userName);

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
