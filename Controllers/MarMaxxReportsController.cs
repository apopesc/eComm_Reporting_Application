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
    public class MarMaxxReportsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<MarMaxxReportsController> _logger;

        private static string myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping.json");
        private static JObject jsonObject = JObject.Parse(myJsonString);

        public static List<ReportTableModel> tableData = new List<ReportTableModel>();
        public static ReportParameterModel reportParams = new ReportParameterModel();
        public static ReportModel selectedReport = new ReportModel();
        public static bool changedReport = false;

        public static List<string> selectedBanners = new List<string>();

        public MarMaxxReportsController(IConfiguration config, ILogger<MarMaxxReportsController> logger)
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

            ReportPageDropdownModel marMaxxDropdownModel = GetFoldersForDropdown();
            
            return View(marMaxxDropdownModel);
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

            if(selectedReportName == "null")
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

            string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Subscription_ID=@ID;";

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
        public JsonResult GetMarMaxxTableData([Bind("reportName,reportFolder")]ReportModel reportData)
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
                else {
                    reportParams = GetReportParameters(reportData);
                    tableData = new List<ReportTableModel>();
                    selectedReport = reportData;
                    selectedBanners = new List<string>();

                    ReportParameterModel reportSpecificParams = new ReportParameterModel();
                    reportSpecificParams.parameters = new List<Parameter>();

                    foreach (Parameter r_param in reportParams.parameters)
                    {
                        reportSpecificParams.parameters.Add(r_param);
                    }

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

                    string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Report_Name=@reportName";

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
                _logger.LogError("Error retrieving MarMaxx table data:  " + e);
                return Json("Error retrieving MarMaxx table data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetMarMaxxTableDataByBanner([Bind("reportName,reportFolder")]ReportModel reportData, List<string> bannerVals)
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
                else if (bannerVals.Count == 0)
                {
                    return Json("Error retrieving table data: Banner is empty");
                }
                else {
                    reportParams = GetReportParameters(reportData);
                    tableData = new List<ReportTableModel>();
                    selectedReport = reportData;
                    selectedBanners = bannerVals;

                    ReportParameterModel reportSpecificParams = new ReportParameterModel();
                    reportSpecificParams.parameters = new List<Parameter>();
                    
                    foreach (Parameter r_param in reportParams.parameters)
                    {
                        reportSpecificParams.parameters.Add(r_param);
                    }

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

                    string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Report_Name=@reportName";

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

                                for(int i=0; i < reportSpecificParams.parameters.Count; i++)
                                {
                                    if( !(dynamicReportParams.ContainsKey(reportSpecificParams.parameters[i].name)))
                                    {
                                        dynamicReportParams[reportSpecificParams.parameters[i].name] = "";
                                    }
                                }

                                tableRow.dynamicParams = dynamicReportParams;

                                if (tableRow.dynamicParams.ContainsKey("Banner"))
                                {
                                    List<string> paramEntries = new List<string>(tableRow.dynamicParams["Banner"].Split(','));
                                    bool containsBanner = bannerVals.Intersect(paramEntries).Any();

                                    if (containsBanner == true)
                                    {
                                        tableData.Add(tableRow);
                                    }
                                }

                            }
                        }
                        connection.Close();
                    }

                    return Json(new { tableParams = reportParams.parameters, rowData = tableData });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving MarMaxx table data:  " + e);
                return Json("Error retrieving MarMaxx table data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInitialTable()
        {
            try
            {
                if(reportParams.parameters != null)
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

                return Json(new { tableParams = reportParams.parameters, rowData = tableData, report = selectedReport, banners = selectedBanners });
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving MarMaxx table data:  " + e);
                return Json("Error retrieving MarMaxx table data: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetMarMaxxReportParameters([Bind("reportName,reportFolder")] ReportModel reportData)
        {
            //ReportDataSource = Netsuite_ODS
            //eCom_ReportDB = eCom_ReportDB
            //ReportServerDB = ReportDatabase
            try
            {
                if(reportData != null && reportData.reportName != "" && reportData.reportFolder != "")
                {
                    reportParams = GetReportParameters(reportData);

                    if (reportData.reportName != selectedReport.reportName)
                    {
                        selectedReport = reportData;
                        changedReport = true;
                    }

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
                    else if (reportParams.dataSource == "ReportServerDB")
                    {
                        connectionstring = configuration.GetConnectionString("ReportServer");
                    }
                    else if (reportParams.dataSource == "eCom_WMSDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_WMSDB");
                    }
                    else if (reportParams.dataSource == "Transportation")
                    {
                        connectionstring = configuration.GetConnectionString("Transportation");
                    }
                    else if (reportParams.dataSource == "VCTool")
                    {
                        connectionstring = configuration.GetConnectionString("VCTool");
                    }
                    else if (reportParams.dataSource == "eCom_RefDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_RefDB");
                    }
                    else if (reportParams.dataSource == "ECD")
                    {
                        connectionstring = configuration.GetConnectionString("ECD");
                    }
                    else if (reportParams.dataSource == "TJX_MFP_Marmaxx")
                    {
                        connectionstring = configuration.GetConnectionString("TJX_MFP_Marmaxx");
                    }

                    for (int i = 0; i < reportParams.parameters.Count; i++)
                    {
                        if ((reportParams.parameters[i].type == "Dropdown" || reportParams.parameters[i].type == "Textbox" || reportParams.parameters[i].type == "MultiDropdown") && (reportParams.parameters[i].queryType == "Stored Procedure"))
                        {
                            SqlConnection connection = new SqlConnection(connectionstring);

                            string procQueryString = "EXEC @storedProc";

                            SqlCommand storedProcQuery = new SqlCommand(procQueryString, connection);
                            storedProcQuery.Parameters.AddWithValue("@storedProc", reportParams.parameters[i].query);

                            //storedProcQuery.CommandType = CommandType.StoredProcedure;

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
        [ValidateAntiForgeryToken]
        public JsonResult SaveMarmaxxReportSubscription([Bind("subscriptionID,subscriptionName,reportName,groupNames,groupIDs,fileFormat,schedule,dynamicParams")] ReportTableModel reportSub)
        {
            try
            {
                if(reportSub.subscriptionName == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Subscription Name field is empty", result = "Error" });
                }
                else if (reportSub.reportName == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Report Name field is empty", result = "Error" });
                }
                else if (reportSub.groupIDs == "" || reportSub.groupNames == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Group field is empty", result = "Error" });
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
                        return Json(new { message = "One or more groups that have been selected have no users tied to them. You can add a user to a group on the User Subscriptions Groups screen.", result = "Error" });
                    }
                    else
                    {
                        connection = new SqlConnection(connectionstring);

                        string addUserQueryString = "INSERT INTO MarMaxxReportSubscriptions (Subscription_Name, Report_Name, Group_Name, Group_ID, Report_Params, File_Format, Schedule) " +
                        "VALUES (@subscriptionName, @reportName, @groupNames, @groupIDs, @paramJson, @fileFormat, @schedule);";

                        SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);
                        addUserQuery.Parameters.AddWithValue("@subscriptionName", reportSub.subscriptionName);
                        addUserQuery.Parameters.AddWithValue("@reportName", reportSub.reportName);
                        addUserQuery.Parameters.AddWithValue("@groupNames", reportSub.groupNames);
                        addUserQuery.Parameters.AddWithValue("@groupIDs", reportSub.groupIDs);
                        addUserQuery.Parameters.AddWithValue("@paramJson", paramJson);
                        addUserQuery.Parameters.AddWithValue("@fileFormat", reportSub.fileFormat);
                        addUserQuery.Parameters.AddWithValue("@schedule", reportSub.schedule);


                        string getSubIDString = "SELECT TOP 1* FROM MarMaxxReportSubscriptions ORDER BY Subscription_ID Desc;"; //Getting the ID by getting the most recently added row
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

                        return Json(new { message = "Success saving subscription: ", result = "Redirect", url = Url.Action("Index", "MarMaxxReports") });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving Marmaxx Report Subscription:  " + e);
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveEditedMarmaxxReportSubscription([Bind("subscriptionID,subscriptionName,reportName,groupNames,groupIDs,fileFormat,schedule,dynamicParams")] ReportTableModel reportSub)
        {
            try
            {

                if (reportSub.subscriptionName == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Subscription Name field is empty", result = "Error"});
                }
                else if (reportSub.reportName == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Report Name field is empty", result = "Error" });
                }
                else if (reportSub.groupIDs == "" || reportSub.groupNames == "")
                {
                    return Json(new { message = "Error Saving Marmaxx Report Subscription: Group field is empty", result = "Error" });
                }
                else
                {
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
                        return Json(new { message = "One or more groups that have been selected have no users tied to them. You can add a user to a group on the User Subscriptions Groups screen.", result = "Error" });
                    }
                    else
                    {
                        connection = new SqlConnection(connectionstring);

                        string editUserQueryString = "UPDATE MarMaxxReportSubscriptions SET Subscription_Name=@subscriptionName, Report_Name=@reportName, Group_Name=@groupNames, Group_ID=@groupIDs, Report_Params=@paramJson, File_Format=@fileFormat, Schedule=@schedule " +
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

                        for (int i = 0; i < tableData.Count; i++)
                        {
                            if (tableData[i].subscriptionID == reportSub.subscriptionID)
                            {
                                tableData[i].subscriptionName = reportSub.subscriptionName;
                                tableData[i].reportName = reportSub.reportName;
                                tableData[i].groupNames = reportSub.groupNames;
                                tableData[i].groupIDs = reportSub.groupIDs;
                                tableData[i].fileFormat = reportSub.fileFormat;
                                tableData[i].schedule = reportSub.schedule;
                                tableData[i].dynamicParams = reportSub.dynamicParams;

                                break;
                            }

                        }

                        return Json(new { message = "Success editing subscription: ", result = "Redirect", url = Url.Action("Index", "MarMaxxReports") });
                    }
                }   
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving Marmaxx Report Subscription:  " + e);
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMarmaxxReportSubscription(int ID)
        {
            try
            {
                if(ID == 0)
                {
                    return Json(new { success = false, message = "Error Deleting Subscription: ID is empty" });
                } 
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "DELETE FROM MarMaxxReportSubscriptions WHERE Subscription_ID=@ID";

                    SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                    deleteUserQuery.Parameters.AddWithValue("@ID", ID);

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
                return Json(new { success = false, message = "Error Deleting Subscription: " + e});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetDepartmentData([Bind("reportName,reportFolder")] ReportModel reportData, List<string> selectedBanners)
        {
            try
            {
                if(selectedBanners.Count == 0)
                {
                    return Json("Error Getting Department Data: Banner field is empty");
                }
                else if(reportData == null || reportData.reportName == "" || reportData.reportFolder == "")
                {
                    return Json("Error Getting Department Data: Report Name or Report Folder is empty");
                }
                else
                {
                    ReportParameterModel local_reportParams = GetReportParameters(reportData);
                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();
                    SqlCommand storedProcQuery = new SqlCommand();

                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("Par_rpt_Departments_by_Banner", connection);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("Par_rpt_Departments_by_Banner", connection);
                    }

                    Parameter departmentParameter = local_reportParams.parameters.Find(x => x.name == "Department_No");
                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    string selectedBannersString = string.Join(",", selectedBanners.ToArray());
                    storedProcQuery.Parameters.AddWithValue("@Banner", selectedBannersString);

                    //Parameter populatedParam = getCascadingDropdownValues(departmentParameter, storedProcQuery, connection);

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    Parameter populatedParam = departmentParameter;

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
                                        if (proc_data_name == departmentParameter.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == departmentParameter.labels[0])
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
                _logger.LogError("Error Getting Department Data: " + e);
                return Json("Error Getting Department Data: " + e);
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

                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_Class_Details_by_Banner", connection);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_rpt_Class_By_Banner", connection);
                    }

                    Parameter classParameter = local_reportParams.parameters.Find(x => x.name == "Class_Number");

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
                        string parsedClass = populatedParam.values[i].Substring(0, charLocation);
                        populatedParam.values[i] = parsedClass;
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
                if(classModel.selectedDepartments.Count == 0)
                {
                    return Json("Error Getting Category Data: Department field is empty");
                }
                else if(classModel.selectedClasses.Count == 0)
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

                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_Category_by_Banner", connection);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_rpt_Category_by_Banner", connection);
                    }

                    Parameter categoryParameter = local_reportParams.parameters.Find(x => x.name == "Category");
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


                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_Brands", connection);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_rpt_Brands", connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Brand_Pattern", brandModel.brandPattern);

                    Parameter brandParameter = local_reportParams.parameters.Find(x => x.name == "Brand");

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


                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_Vendors", connection);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                        storedProcQuery = new SqlCommand("par_rpt_Vendors", connection);
                    }

                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    storedProcQuery.Parameters.AddWithValue("@Vendor_Pattern", vendorModel.vendorPattern);

                    Parameter vendorParameter = local_reportParams.parameters.Find(x => x.name == "Vendor");

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
        public JsonResult GetBannersForReport([Bind("reportName,reportFolder")] ReportModel reportData)
        {
            try
            {
                ReportParameterModel local_reportParams = GetReportParameters(reportData);
                bool hasBanner = false;
               
                Parameter bannerParam = new Parameter();
                Parameter populatedParam = new Parameter();

                foreach (Parameter param in local_reportParams.parameters)
                {
                    if(param.name == "Banner")
                    {
                        bannerParam = param;
                        hasBanner = true;
                        break;
                    }
                }

                if(hasBanner == true)
                {
                    string connectionstring = "";
                    SqlConnection connection = new SqlConnection();

                    //There are other data sources that need to be mapped here
                    if (local_reportParams.dataSource == "ReportDataSource")
                    {
                        connectionstring = configuration.GetConnectionString("NetSuite_DB");
                        connection = new SqlConnection(connectionstring);
                    }
                    else if (local_reportParams.dataSource == "eCom_ReportDB")
                    {
                        connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                        connection = new SqlConnection(connectionstring);
                    }

                    SqlCommand storedProcQuery = new SqlCommand("par_Banner", connection);
                    storedProcQuery.CommandType = CommandType.StoredProcedure;

                    List<string> dropdownValues = new List<string>();
                    List<string> dropdownLabels = new List<string>();

                    populatedParam = bannerParam;

                    //populatedBannerParam = getCascadingDropdownValues(bannerParam, storedProcQuery, connection);
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
                                        if (proc_data_name == bannerParam.values[0])
                                        {
                                            var proc_val = stored_proc_reader.GetValue(j);

                                            string dropdownVal = proc_val.ToString();
                                            dropdownValues.Add(dropdownVal);
                                        }

                                        if (proc_data_name == bannerParam.labels[0])
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

                }
                                
                return Json(populatedParam);
            }
            catch (Exception e)
            {
                _logger.LogError("Error Getting Banner Data: " + e);
                return Json("Error Getting Banner Data: " + e);
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
                _logger.LogError("Error retrieving MarMaxx report parameters: " + e);
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


        //public Parameter getCascadingDropdownValues(Parameter cascadingParam , SqlCommand storedProcQuery, SqlConnection connection)
        //{
        //    List<string> dropdownValues = new List<string>();
        //    List<string> dropdownLabels = new List<string>();

        //    Parameter populatedParam = cascadingParam;

        //    using (connection)
        //    {
        //        connection.Open();
        //        using (SqlDataReader stored_proc_reader = storedProcQuery.ExecuteReader())
        //        {
        //            while (stored_proc_reader.Read())
        //            {
        //                var proc_data_length = stored_proc_reader.FieldCount;

        //                if (proc_data_length > 1)
        //                {
        //                    for (int j = 0; j < proc_data_length; j++)
        //                    {
        //                        var proc_data_name = stored_proc_reader.GetName(j);
        //                        if (proc_data_name == cascadingParam.values[0])
        //                        {
        //                            var proc_val = stored_proc_reader.GetValue(j);

        //                            string dropdownVal = proc_val.ToString();
        //                            dropdownValues.Add(dropdownVal);
        //                        }

        //                        if (proc_data_name == cascadingParam.labels[0])
        //                        {
        //                            var proc_label = stored_proc_reader.GetValue(j);

        //                            string dropdownLab = proc_label.ToString();
        //                            dropdownLabels.Add(dropdownLab);
        //                        }
        //                    }
        //                }
        //                else //if only one column is returned from the stored procedure, put in both labels and values
        //                {
        //                    var proc_val = stored_proc_reader.GetValue(0);

        //                    string dropdownEntry = proc_val.ToString();
        //                    dropdownValues.Add(dropdownEntry);
        //                    dropdownLabels.Add(dropdownEntry);
        //                }
        //            }
        //        }
        //        connection.Close();
        //    }

        //    populatedParam.values = dropdownValues;
        //    populatedParam.labels = dropdownLabels;

        //    return populatedParam;
        //}
    }
}
