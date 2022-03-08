﻿using Microsoft.AspNetCore.Mvc;
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

namespace eComm_Reporting_Application.Controllers
{
    [Authorize]
    public class MarMaxxReportsController : Controller
    {
        private readonly IConfiguration configuration;
        private static string myJsonString = System.IO.File.ReadAllText("JSON Report Parameter Mapping.json");
        private static JObject jsonObject = JObject.Parse(myJsonString);

        public static List<ReportTableModel> tableData = new List<ReportTableModel>();
        public static ReportParameterModel reportParams = new ReportParameterModel();
        public static ReportModel selectedReport = new ReportModel();
        public static bool changedReport = false;

        public MarMaxxReportsController(IConfiguration config)
        {
            this.configuration = config;
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

            string queryString = "SELECT * FROM MarMaxxReportSubscriptions WHERE Subscription_ID='" + ID + "'";

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlCommand getSubscriptionData = new SqlCommand(queryString, connection);
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

                return Json(new { tableParams = reportParams.parameters, rowData = tableData});

            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetInitialTable()
        {
            try
            {
                if(reportParams.parameters != null)
                {
                    if (reportParams.parameters[0].name != "Subscription_ID")
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
                
                return Json(new { tableParams = reportParams.parameters, rowData = tableData, report = selectedReport });
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
            //ReportServerDB = ReportDatabase
            try
            {
                reportParams = GetReportParameters(reportData);

                if(reportData.reportName != selectedReport.reportName)
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
                    if ( (reportParams.parameters[i].type == "Dropdown" || reportParams.parameters[i].type == "Textbox" || reportParams.parameters[i].type == "MultiDropdown") && (reportParams.parameters[i].queryType == "Stored Procedure" || reportParams.parameters[i].queryType == "In Line"))
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

                            if(reportParams.parameters[i].name != "Department_No" && reportParams.parameters[i].name != "Class_Number" && reportParams.parameters[i].name != "Category") //these parameters take values from banner and each other to return data.
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
            catch (Exception e)
            {
                return Json("Error retrieving report parameters: " + e);
            }

        }

        [HttpPost]
        public JsonResult SaveMarmaxxReportSubscription(ReportTableModel reportSub)
        {
            try
            {
                int subscriptionID = 0;

                string paramJson = JsonConvert.SerializeObject(reportSub.dynamicParams);
                //Add query here to store in database, store group ID in their respective columns, and paramJson in the last column

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                string addUserQueryString = "INSERT INTO MarMaxxReportSubscriptions (Subscription_Name, Report_Name, Group_Name, Group_ID, Report_Params, File_Format, Schedule) " +
                    "VALUES ('" + reportSub.subscriptionName + "', '" + reportSub.reportName + "', '" + reportSub.groupNames + "', '" + reportSub.groupIDs + "', '" + paramJson + "', '" + reportSub.fileFormat + "', '" + reportSub.schedule + "');";

                SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);

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

                if(changedReport == true)
                {
                    tableData = new List<ReportTableModel>();
                    changedReport = false;
                }

                tableData.Insert(0, newEntry); //Adding new subscription to start of table


                return Json(new { message = "Success saving subscription: ", result = "Redirect", url = Url.Action("Index", "MarMaxxReports") });
            }
            catch (Exception e)
            {
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        [HttpPost]
        public JsonResult SaveEditedMarmaxxReportSubscription(ReportTableModel reportSub)
        {
            try
            {
                string paramJson = JsonConvert.SerializeObject(reportSub.dynamicParams);
                //Add query here to store in database, store group ID in their respective columns, and paramJson in the last column

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                string editUserQueryString = "UPDATE MarMaxxReportSubscriptions SET Subscription_Name='" + reportSub.subscriptionName + "', Report_Name='" + reportSub.reportName + "', Group_Name='" + reportSub.groupNames + "', Group_ID='" + reportSub.groupIDs + "', Report_Params='" + paramJson + "', File_Format='" + reportSub.fileFormat + "', Schedule='" + reportSub.schedule + "' " +
                    "WHERE Subscription_ID="+ reportSub.subscriptionID+";";

                SqlCommand editUserQuery = new SqlCommand(editUserQueryString, connection);

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
            catch (Exception e)
            {
                return Json("Error Saving Marmaxx Report Subscription: " + e);
            }
        }

        [HttpPost]
        public JsonResult DeleteMarmaxxReportSubscription(int ID)
        {
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "DELETE FROM MarMaxxReportSubscriptions WHERE Subscription_ID=" + ID;

                SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                using (connection)
                {
                    connection.Open();
                    SqlDataReader reader = deleteUserQuery.ExecuteReader();
                    connection.Close();
                }

                return Json("Success Deleting Subscription: ");
            }
            catch (Exception e)
            {
                return Json("Error Deleting Subscription: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetDepartmentData(ReportModel reportData, List<string> selectedBanners)
        {
            try
            {
                ReportParameterModel local_reportParams = GetReportParameters(reportData);
                string connectionstring = "";

                //There are other data sources that need to be mapped here
                if (local_reportParams.dataSource == "ReportDataSource")
                {
                    connectionstring = configuration.GetConnectionString("NetSuite_DB");
                }
                else if (local_reportParams.dataSource == "eCom_ReportDB")
                {
                    connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                }

                Parameter departmentParameter = local_reportParams.parameters.Find(x => x.name == "Department_No");

                SqlConnection connection = new SqlConnection(connectionstring);
                SqlCommand storedProcQuery = new SqlCommand(departmentParameter.query, connection);
                storedProcQuery.CommandType = CommandType.StoredProcedure;

                string selectedBannersString = string.Join(",", selectedBanners.ToArray());
                storedProcQuery.Parameters.AddWithValue("@Banner", selectedBannersString);

                Parameter populatedParam = getCascadingDropdownValues(departmentParameter, storedProcQuery, connection);
                
                return Json(populatedParam);
            }
            catch (Exception e)
            {
                return Json("Error Deleting Subscription: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetClassData([FromBody] DepartmentModel departmentModel)
        {
            try
            {
                ReportParameterModel local_reportParams = GetReportParameters(departmentModel.reportData);
                string connectionstring = "";

                //There are other data sources that need to be mapped here
                if (local_reportParams.dataSource == "ReportDataSource")
                {
                    connectionstring = configuration.GetConnectionString("NetSuite_DB");
                }
                else if (local_reportParams.dataSource == "eCom_ReportDB")
                {
                    connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                }

                Parameter classParameter = local_reportParams.parameters.Find(x => x.name == "Class_Number");

                SqlConnection connection = new SqlConnection(connectionstring);
                SqlCommand storedProcQuery = new SqlCommand(classParameter.query, connection);
                storedProcQuery.CommandType = CommandType.StoredProcedure;

                string selectedDepartmentsString = string.Join(",", departmentModel.selectedDepartments.ToArray());
                storedProcQuery.Parameters.AddWithValue("@Department_No", selectedDepartmentsString);

                Parameter populatedParam = getCascadingDropdownValues(classParameter, storedProcQuery, connection);

                return Json(populatedParam);
            }
            catch (Exception e)
            {
                return Json("Error Deleting Subscription: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetCategoryData([FromBody]ClassModel classModel)
        {
            try
            {
                ReportParameterModel local_reportParams = GetReportParameters(classModel.reportData);
                string connectionstring = "";

                //There are other data sources that need to be mapped here
                if (local_reportParams.dataSource == "ReportDataSource")
                {
                    connectionstring = configuration.GetConnectionString("NetSuite_DB");
                }
                else if (local_reportParams.dataSource == "eCom_ReportDB")
                {
                    connectionstring = configuration.GetConnectionString("eCom_ReportDB");
                }

                Parameter categoryParameter = local_reportParams.parameters.Find(x => x.name == "Category");

                SqlConnection connection = new SqlConnection(connectionstring);
                SqlCommand storedProcQuery = new SqlCommand(categoryParameter.query, connection);
                storedProcQuery.CommandType = CommandType.StoredProcedure;

                string selectedDepartmentsString = string.Join(",", classModel.selectedDepartments.ToArray());
                storedProcQuery.Parameters.AddWithValue("@Department_No", selectedDepartmentsString);

                string selectedClassesString = string.Join(",", classModel.selectedClasses.ToArray());
                storedProcQuery.Parameters.AddWithValue("@Class_Number", selectedClassesString);
                storedProcQuery.CommandTimeout = 350;

                Parameter populatedParam = getCascadingDropdownValues(categoryParameter, storedProcQuery, connection);

                return Json(populatedParam);
            }
            catch (Exception e)
            {
                return Json("Error Getting Category Data: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetBrandData([FromBody] BrandModel brandModel)
        {
            try
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

                Parameter populatedParam = getCascadingDropdownValues(brandParameter, storedProcQuery, connection);

                return Json(populatedParam);
            }
            catch (Exception e)
            {
                return Json("Error Getting Brand Data: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetVendorData([FromBody] VendorModel vendorModel)
        {
            try
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

                Parameter populatedParam = getCascadingDropdownValues(vendorParameter, storedProcQuery, connection);

                return Json(populatedParam);
            }
            catch (Exception e)
            {
                return Json("Error Getting Vendor Data: " + e);
            }
        }

        

        public Parameter getCascadingDropdownValues(Parameter cascadingParam , SqlCommand storedProcQuery, SqlConnection connection)
        {
            List<string> dropdownValues = new List<string>();
            List<string> dropdownLabels = new List<string>();

            Parameter populatedParam = cascadingParam;

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
                                if (proc_data_name == cascadingParam.values[0])
                                {
                                    var proc_val = stored_proc_reader.GetValue(j);

                                    string dropdownVal = proc_val.ToString();
                                    dropdownValues.Add(dropdownVal);
                                }

                                if (proc_data_name == cascadingParam.labels[0])
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

            return populatedParam;
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
