﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using eComm_Reporting_Application.Extensions;

namespace eComm_Reporting_Application.Controllers
{
    [Authorize]
    public class SubscriptionGroupsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<SubscriptionGroupsController> _logger;

        //public static List<UserSubscriptionTableModel> tableData = new List<UserSubscriptionTableModel>();
        //public static List<string> selectedMasterGroups = new List<string>();
        //public static List<string> selectedGroups = new List<string>();
        //public static List<string> selectedGroupIDs = new List<string>();


        public SubscriptionGroupsController(IConfiguration config, ILogger<SubscriptionGroupsController> logger)
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

            if(isAuthenticated == false) {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            UserSubscriptionDropdownModel subModel = GetFilterData(); 
            
            return View(subModel);
        }


        public IActionResult AddNewUserSub()
        {
            bool isAuthenticated = isAuthenticatedUser();

            if (isAuthenticated == false)
            {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            UserSubscriptionDropdownModel subModel = GetFilterData();

            return View(subModel);
        }

        public IActionResult EditUserSub(int ID)
        {
            bool isAuthenticated = isAuthenticatedUser();

            if (isAuthenticated == false)
            {
                string userName = User.Identity.Name;
                string error = "User " + userName + " does not have sufficient permissions to access this application. Please contact an administrator.";
                return RedirectToAction("Error", new { errorMsg = error });
            }

            EditUserDropdownModel subModel = new EditUserDropdownModel();

            List<string> master_groups_list = new List<string>();

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);

            string masterGroupsQueryString = "SELECT DISTINCT MasterGroup FROM Groups WHERE MasterGroup IS NOT NULL";
            string userQueryString = "SELECT * FROM UserSubscriptions WHERE ID=@ID";

            SqlCommand masterGroupsQuery = new SqlCommand(masterGroupsQueryString, connection);
            SqlCommand userQuery = new SqlCommand(userQueryString, connection);

            userQuery.Parameters.AddWithValue("@ID", ID);

            using (connection)
            {
                connection.Open();
                using (SqlDataReader reader = masterGroupsQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var groupString = reader.GetString(0);
                        master_groups_list.Add(groupString);
                    }
                }

                using (SqlDataReader reader = userQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subModel.userEmail = reader.GetString(1);
                        subModel.isActive = reader.GetString(2);
                        subModel.selectedGroupNames = reader.GetString(3);
                        subModel.selectedGroupIDs = reader.GetString(4);
                        subModel.selectedMasterGroups = reader.GetString(5);
                    }
                }
                connection.Close();
            }

            subModel.ID = ID;
            subModel.masterGroupsList = master_groups_list;

            return View(subModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUserSubToDB(string userEmail, string isActive, string selectedGroupIDs, string selectedGroups, string selectedMasterGroups)
        {
            try
            {
                int userID = 0;
                bool isValidEmail = false;

                if(userEmail != "")
                {
                    int substringIndex = userEmail.IndexOf("@");

                    if(substringIndex != -1)
                    {
                        string emailDomain = userEmail.Substring(substringIndex + 1);
                        if (emailDomain == "tjx.com" || emailDomain == "tjxcanada.ca" || emailDomain == "tjxeurope.com")
                        {
                            isValidEmail = true;
                        }
                    }

                }

                if(isValidEmail == false)
                {
                    return Json("Error Saving to Database: User Email field is empty, or not a TJX email.");
                }
                else if(isActive == "")
                {
                    return Json("Error Saving to Database: Is Active field is empty.");
                }
                else if(selectedGroupIDs == "" || selectedGroups == "")
                {
                    return Json("Error Saving to Database: Group field is empty.");
                }
                else if (selectedMasterGroups == "")
                {
                    return Json("Error Saving to Database: Master Group field is empty.");
                }
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);

                    string addUserQueryString = "INSERT INTO UserSubscriptions (User_Email, Is_Active, User_Group, Group_ID, Master_Group) " +
                        "VALUES (@userEmail, @isActive, @selectedGroups, @selectedGroupIDs, @selectedMasterGroups);";

                    SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);
                    addUserQuery.Parameters.AddWithValue("@userEmail", userEmail);
                    addUserQuery.Parameters.AddWithValue("@isActive", isActive);
                    addUserQuery.Parameters.AddWithValue("@selectedGroups", selectedGroups);
                    addUserQuery.Parameters.AddWithValue("@selectedGroupIDs", selectedGroupIDs);
                    addUserQuery.Parameters.AddWithValue("@selectedMasterGroups", selectedMasterGroups);

                    string getUserIDString = "SELECT TOP 1* FROM UserSubscriptions ORDER BY ID Desc;"; //Getting the ID by getting the most recently added row

                    SqlCommand getUserID = new SqlCommand(getUserIDString, connection);

                    using (connection)
                    {
                        connection.Open();
                        using SqlDataReader reader = addUserQuery.ExecuteReader();
                        connection.Close();

                        connection.Open();
                        using (SqlDataReader reader_ID = getUserID.ExecuteReader())
                        {
                            while (reader_ID.Read())
                            {
                                var id = reader_ID.GetInt32(0);
                                userID = id;
                            }
                        }

                        connection.Close();
                    }

                    UserSubscriptionTableModel newEntry = new UserSubscriptionTableModel();
                    newEntry.ID = userID;
                    newEntry.userEmail = userEmail;
                    newEntry.isActive = isActive;
                    newEntry.groupID = selectedGroupIDs;
                    newEntry.group = selectedGroups;
                    newEntry.masterGroup = selectedMasterGroups;


                    List<UserSubscriptionTableModel> currentTable = HttpContext.Session.GetObjectFromJson<List<UserSubscriptionTableModel>>("userSubTableData");

                    if(currentTable == null)
                    {
                        currentTable = new List<UserSubscriptionTableModel>();
                    }

                    currentTable.Insert(0, newEntry); //Adding new user to start of table

                    HttpContext.Session.SetObjectAsJson<List<UserSubscriptionTableModel>>("userSubTableData", currentTable);

                    return Json(new { result = "Redirect", url = Url.Action("Index", "SubscriptionGroups") });
                }

               
            }
            catch (Exception e)
            {
                _logger.LogError("Error Saving to Database: " + e);
                return Json("Error Saving to Database: " + e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetTableData([Bind("isActive,groupsIDList,groupsList,masterGroupsList")] UserSubscriptionDropdownModel filterData)
        {
            try
            {

                List<UserSubscriptionTableModel> newUserTableData = new List<UserSubscriptionTableModel>();

                HttpContext.Session.SetObjectAsJson<List<string>>("userGroupIDsList", filterData.groupsIDList);
                HttpContext.Session.SetObjectAsJson<List<string>>("userGroupsList", filterData.groupsList);
                HttpContext.Session.SetObjectAsJson<List<string>>("userMasterGroupsList", filterData.masterGroupsList);

                if(filterData.groupsIDList == null || filterData.groupsList == null)
                {
                    return Json("Error retrieving table data: Group field is empty");
                } 
                else if (filterData.masterGroupsList == null)
                {
                    return Json("Error retrieving table data: Master Group field is empty");
                }
                else if (filterData.isActive == 0)
                {
                    return Json("Error retrieving table data: Is Active field is empty");
                }
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);
                    SqlCommand tableQuery = new SqlCommand();

                    string queryString = "SELECT * FROM UserSubscriptions WHERE Is_Active IN ";

                    if (filterData.isActive == 1)
                    {
                        queryString = queryString + "('Y')";
                    }
                    else if (filterData.isActive == 2)
                    {
                        queryString = queryString + "('N')";
                    }
                    else //If both are selected, show all data for isActive
                    {
                        queryString = queryString + "('Y', 'N')";
                    }

                   queryString = queryString + " AND (Group_ID LIKE ";

                    for (int i = 0; i < filterData.groupsIDList.Count; i++)
                    {
                        string currentParam = string.Format("@GroupID{0}", i);

                        if (i == 0)
                        {
                            queryString = queryString + currentParam;
                        }
                        else
                        {
                            queryString = queryString + " OR Group_ID LIKE " + currentParam; 
                        }

                        tableQuery.Parameters.AddWithValue(currentParam, "%" + filterData.groupsIDList[i] + "%");
                    }

                    queryString = queryString + ") AND (Master_Group LIKE ";

                    for (int i = 0; i < filterData.masterGroupsList.Count; i++)
                    {
                        string currentParam = string.Format("@MasterGroup{0}", i);

                        if (i == 0)
                        {
                            queryString = queryString + currentParam;
                        }
                        else
                        {
                            queryString = queryString + " OR Master_Group LIKE " + currentParam;
                        }

                        tableQuery.Parameters.AddWithValue(currentParam, "%" + filterData.masterGroupsList[i] + "%");
                    }

                    queryString = queryString + ");";

                    tableQuery.CommandText = queryString;
                    tableQuery.Connection = connection;

                    using (connection)
                    {
                        connection.Open();
                        using (SqlDataReader reader = tableQuery.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserSubscriptionTableModel entry = new UserSubscriptionTableModel();
                                entry.ID = reader.GetInt32(0);
                                entry.userEmail = reader.GetString(1);
                                entry.isActive = reader.GetString(2);
                                entry.group = reader.GetString(3);
                                entry.groupID = reader.GetString(4);
                                entry.masterGroup = reader.GetString(5);
                                newUserTableData.Add(entry);
                            }
                        }
                        connection.Close();
                    }

                    HttpContext.Session.SetObjectAsJson<List<UserSubscriptionTableModel>>("userSubTableData", newUserTableData);

                    //Returning the table data to the front end
                    return Json(newUserTableData);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving table data: " + e);
                return Json("Error retrieving table data: " + e);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetInitialTable()
        {
            try
            {
                List<UserSubscriptionTableModel> tableData = HttpContext.Session.GetObjectFromJson<List<UserSubscriptionTableModel>>("userSubTableData");
                List<string> selectedMasterGroups = HttpContext.Session.GetObjectFromJson<List<string>>("userMasterGroupsList");
                List<string> selectedGroups = HttpContext.Session.GetObjectFromJson<List<string>>("userGroupsList");
                List<string> selectedGroupIDs = HttpContext.Session.GetObjectFromJson<List<string>>("userGroupIDsList");

                if (tableData == null)
                {
                    tableData = new List<UserSubscriptionTableModel>();
                }


                return Json(new { tableData, selectedGroupIDs, selectedGroups, selectedMasterGroups } );
            }
            catch (Exception e)
            {
                _logger.LogError("Error retrieving table data: " + e);
                return Json("Error retrieving table data: " + e);
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public JsonResult EditUserSub(List<UserSubscriptionTableModel> editedUsersList)
        //{
        //    try
        //    {
        //        string successString = "Success editing users: ";

        //        for (int i = 0; i < editedUsersList.Count; i++)
        //        {
        //            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
        //            SqlConnection connection = new SqlConnection(connectionstring);

        //            string queryString = "Update UserSubscriptions SET User_Email=@userEmail, Is_Active=@isActive, User_Group=@group" +
        //                ", Group_ID=@groupID, Master_Group=@masterGroup WHERE ID=@ID;";

        //            SqlCommand editUserQuery = new SqlCommand(queryString, connection);
        //            editUserQuery.Parameters.AddWithValue("@userEmail", editedUsersList[i].userEmail);
        //            editUserQuery.Parameters.AddWithValue("@isActive", editedUsersList[i].isActive);
        //            editUserQuery.Parameters.AddWithValue("@group", editedUsersList[i].group);
        //            editUserQuery.Parameters.AddWithValue("@groupID", editedUsersList[i].groupID);
        //            editUserQuery.Parameters.AddWithValue("@masterGroup", editedUsersList[i].masterGroup);
        //            editUserQuery.Parameters.AddWithValue("@ID", editedUsersList[i].ID);

        //            using (connection)
        //            {
        //                connection.Open();
        //                SqlDataReader reader = editUserQuery.ExecuteReader();
        //                connection.Close();
        //            }
        //            successString = successString + editedUsersList[i].userEmail + ", ";
        //        }

        //        successString = successString.Substring(0, successString.Length - 2);
        //        return Json(successString);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError("Error editing user: " + e);
        //        return Json("Error editing user: " + e);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteUserSub(int ID)
        {
            try
            {
                if(ID != 0)
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "DELETE FROM UserSubscriptions WHERE ID=@ID";

                    SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                    deleteUserQuery.Parameters.AddWithValue("@ID", ID);

                    using (connection)
                    {
                        connection.Open();
                        SqlDataReader reader = deleteUserQuery.ExecuteReader();
                        connection.Close();
                    }

                    List<UserSubscriptionTableModel> currentTable = HttpContext.Session.GetObjectFromJson<List<UserSubscriptionTableModel>>("userSubTableData");
                    currentTable.RemoveAll(x => x.ID == ID);
                    HttpContext.Session.SetObjectAsJson<List<UserSubscriptionTableModel>>("userSubTableData", currentTable);

                    return Json(new { success = true, message = "Success Deleting User: " });
                }
                else
                {
                    return Json(new { success = false, message = "Error Deleting User: ID is empty" });
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError("Error Deleting Report Subscription: " + e);
                return Json(new { success = false, message = "Error Deleting Subscription: " + e });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetGroupValues(List<string> masterGroupList)
        {
            try
            {
                if(masterGroupList.Count < 1)
                {
                    _logger.LogError("Error getting group values: Master Group is Empty");
                    return Json(new { success = false, message = "Error getting group values: Master Group is Empty" });
                } 
                else
                {
                    IDictionary<string, string> groups = new Dictionary<string, string>();

                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    var masterGroupParams = new string[masterGroupList.Count];
                    SqlCommand groupsQuery = new SqlCommand();
                    for (int i = 0; i < masterGroupList.Count; i++)
                    {
                        masterGroupParams[i] = string.Format("@MasterGroup{0}", i);
                        groupsQuery.Parameters.AddWithValue(masterGroupParams[i], masterGroupList[i]);
                    }

                    groupsQuery.CommandText = string.Format("SELECT GroupID,GroupName FROM Groups WHERE MasterGroup IN ({0})", string.Join(",", masterGroupParams));
                    groupsQuery.Connection = connection;

                    using (connection)
                    {
                        connection.Open();
                        using (SqlDataReader reader = groupsQuery.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var groupID = reader.GetString(0);
                                var groupName = reader.GetString(1);
                                groups.Add(groupID, groupName);
                            }
                        }
                    }
                    return Json(new { success = true, groups = groups });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting group values: " + e);
                return Json(new { success = false, message = "Error getting group values: " + e });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUserSubToDB(int ID, string userEmail, string isActive, string selectedGroupIDs, string selectedGroups, string selectedMasterGroups)
        {
            try
            {
                bool isValidEmail = false;

                if (userEmail != "")
                {
                    int substringIndex = userEmail.IndexOf("@");

                    if (substringIndex != -1)
                    {
                        string emailDomain = userEmail.Substring(substringIndex + 1);
                        if (emailDomain == "tjx.com" || emailDomain == "tjxcanada.ca" || emailDomain == "tjxeurope.com")
                        {
                            isValidEmail = true;
                        }
                    }

                }

                if (isValidEmail == false)
                {
                    return Json("Error Saving to Database: User Email field is empty, or not a TJX email.");
                }
                else if (isActive == "")
                {
                    return Json("Error Saving to Database: Is Active field is empty.");
                }
                else if (selectedGroupIDs == "" || selectedGroups == "")
                {
                    return Json("Error Saving to Database: Group field is empty.");
                }
                else if (selectedMasterGroups == "")
                {
                    return Json("Error Saving to Database: Master Group field is empty.");
                }
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);

                    string editUserQueryString = "UPDATE UserSubscriptions SET User_Email=@userEmail, Is_Active=@isActive, User_Group=@group, Group_ID=@groupID, Master_Group=@masterGroup " +
                        "WHERE ID=@ID;";

                    SqlCommand editUserQuery = new SqlCommand(editUserQueryString, connection);
                    editUserQuery.Parameters.AddWithValue("@userEmail", userEmail);
                    editUserQuery.Parameters.AddWithValue("@isActive", isActive);
                    editUserQuery.Parameters.AddWithValue("@group", selectedGroups);
                    editUserQuery.Parameters.AddWithValue("@groupID", selectedGroupIDs);
                    editUserQuery.Parameters.AddWithValue("@masterGroup", selectedMasterGroups);
                    editUserQuery.Parameters.AddWithValue("@ID", ID);

                    using (connection)
                    {
                        connection.Open();
                        using SqlDataReader reader = editUserQuery.ExecuteReader();
                        connection.Close();
                    }

                    List<UserSubscriptionTableModel> currentTable = HttpContext.Session.GetObjectFromJson<List<UserSubscriptionTableModel>>("userSubTableData");
                    for (int i = 0; i < currentTable.Count; i++)
                    {
                        if (currentTable[i].ID == ID)
                        {
                            currentTable[i].userEmail = userEmail;
                            currentTable[i].isActive = isActive;
                            currentTable[i].groupID = selectedGroupIDs;
                            currentTable[i].group = selectedGroups;
                            currentTable[i].masterGroup = selectedMasterGroups;
                            break;
                        }
                    }
                    HttpContext.Session.SetObjectAsJson<List<UserSubscriptionTableModel>>("userSubTableData", currentTable);


                    return Json(new { result = "Redirect", url = Url.Action("Index", "SubscriptionGroups") });
                }
            }
            catch (Exception e)
            {
                return Json("Error Saving to Database: " + e);
            }
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

        //Getting filter data from the database
        private UserSubscriptionDropdownModel GetFilterData()
        {
            int is_active = 0;
            List<string> groups_list = new List<string>();
            List<string> groupsID_list = new List<string>();
            List<string> master_groups_list = new List<string>();

            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string groupIDsQueryString = "SELECT * FROM Groups";
                string masterGroupsQueryString = "SELECT * FROM MasterGroups";

                SqlCommand groupIDsQuery = new SqlCommand(groupIDsQueryString, connection);
                SqlCommand masterGroupsQuery = new SqlCommand(masterGroupsQueryString, connection);

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
                    using (SqlDataReader reader = masterGroupsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var masterGroupString = reader.GetString(1);
                            master_groups_list.Add(masterGroupString);
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                //Redirect to error page and pass forward exception e once error page is set up.
            }

            UserSubscriptionDropdownModel filterDataModel = new UserSubscriptionDropdownModel()
            {
                isActive = is_active,
                groupsIDList = groupsID_list,
                groupsList = groups_list,
                masterGroupsList = master_groups_list
            };

            return filterDataModel;
        }
    }
}