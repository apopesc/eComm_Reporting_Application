using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace eComm_Reporting_Application.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly ILogger<AdminController> _logger;

        public static AdminPageModel adminModel = new AdminPageModel();


        public AdminController(IConfiguration config, ILogger<AdminController> logger)
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

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);

            string groupsQueryString = "SELECT * FROM Groups";
            SqlCommand groupsQuery = new SqlCommand(groupsQueryString, connection);

            string masterGroupsQueryString = "SELECT * FROM MasterGroups";
            SqlCommand masterGroupsQuery = new SqlCommand(masterGroupsQueryString, connection);

            List<GroupModel> groupsList = new List<GroupModel>();
            List<string> masterGroupsList = new List<string>();

            using (connection)
            {
                connection.Open();
                using (SqlDataReader reader = groupsQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GroupModel groupEntry = new GroupModel();
                        groupEntry.groupID = reader.GetString(0);
                        groupEntry.groupName = reader.GetString(1);
                        groupEntry.masterGroup = reader.GetString(2);
                        groupsList.Add(groupEntry);
                    }
                }

                using (SqlDataReader reader = masterGroupsQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //var mastergroupID = reader.GetString(0);
                        var mastergroupName = reader.GetString(1);
                        masterGroupsList.Add(mastergroupName);
                    }
                }
                connection.Close();
            }

            adminModel.masterGroupsList = masterGroupsList;
            adminModel.groupsList = groupsList;
            return View(adminModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewGroup(string masterGroup, string groupID, string groupName)
        {
            try
            {
                if (masterGroup == null || masterGroup == "")
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: Master Group is empty." });
                }
                else if (groupID == null || groupID == "")
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: Group ID is empty." });
                }
                else if (groupName == null || groupName == "")
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: Group is empty." });
                }
                else {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);

                    string addGroupQueryString = "INSERT INTO Groups (GroupID, GroupName, MasterGroup) " +
                        "VALUES (@groupID, @groupName, @masterGroup);";

                    SqlCommand addGroupQuery = new SqlCommand(addGroupQueryString, connection);

                    addGroupQuery.Parameters.AddWithValue("@groupID", groupID);
                    addGroupQuery.Parameters.AddWithValue("@groupName", groupName);
                    addGroupQuery.Parameters.AddWithValue("@masterGroup", masterGroup);

                    using (connection)
                    {
                        connection.Open();
                        using SqlDataReader reader = addGroupQuery.ExecuteReader();
                        connection.Close();

                    }

                    GroupModel newGroup = new GroupModel();
                    newGroup.masterGroup = masterGroup;
                    newGroup.groupID = groupID;
                    newGroup.groupName = groupName;

                    adminModel.groupsList.Insert(0, newGroup);

                    _logger.LogTrace("New Subscription Group Added. Group ID:" + groupID + ", Group Name:" + groupName + ", Associated Master Group:" + masterGroup);
                    return Json(new { success = true, saved_masterGroup = masterGroup, new_groupID = groupID, new_groupName = groupName });
                }
            }
            catch (Exception e)
            {
                string exceptionString = e.ToString();
                if (exceptionString.Contains("Violation of UNIQUE KEY constraint"))
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: The entered Group ID already exists."});
                }
                else
                {
                    _logger.LogError("Error saving new group: " + e);
                    return Json(new { success = false, errorMsg = "Error saving new group: " + e});
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteGroup(string groupID)
        {
            try
            {
                if (groupID == null || groupID == "")
                {
                    return Json(new { success = false, message = "Error Deleting Group: Group ID is empty" });
                }
                else {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "DELETE FROM Groups WHERE GroupID=@groupID;";

                    SqlCommand deleteGroupQuery = new SqlCommand(queryString, connection);

                    deleteGroupQuery.Parameters.AddWithValue("@groupID", groupID);

                    using (connection)
                    {
                        connection.Open();
                        SqlDataReader reader = deleteGroupQuery.ExecuteReader();
                        connection.Close();
                    }
                    return Json(new { success = true, message = "Success Deleting Group: " });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error Deleting Group: " + e);
                return Json(new { success = false, message = "Error Deleting Group: " + e });
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewMasterGroup(string masterGroup)
        {
            try
            {
                if(masterGroup == null || masterGroup == "")
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: The entered Master Group is empty." });
                }
                else
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                    SqlConnection connection = new SqlConnection(connectionstring);

                    string addMasterGroupQueryString = "INSERT INTO MasterGroups (MasterGroup) " +
                        "VALUES (@masterGroup);";

                    SqlCommand addMasterGroupQuery = new SqlCommand(addMasterGroupQueryString, connection);
                    addMasterGroupQuery.Parameters.AddWithValue("@masterGroup", masterGroup);

                    using (connection)
                    {
                        connection.Open();
                        using SqlDataReader reader = addMasterGroupQuery.ExecuteReader();
                        connection.Close();

                    }

                    adminModel.masterGroupsList.Insert(0, masterGroup);

                    return Json(new { success = true, saved_masterGroup = masterGroup });
                }
            }
            catch (Exception e)
            {
                string exceptionString = e.ToString();
                if (exceptionString.Contains("Violation of UNIQUE KEY constraint"))
                {
                    return Json(new { success = false, errorMsg = "Error saving new group: The entered Master Group already exists." });
                }
                else
                {
                    _logger.LogError("Error saving new master group: " + e);
                    return Json(new { success = false, errorMsg = "Error saving new master group: " + e });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteMasterGroup(string masterGroup)
        {
            try
            {
                if (masterGroup == null || masterGroup == "")
                {
                    return Json(new { success = false, message = "Error Deleting Master Group: Master Group is empty" });
                }
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "DELETE FROM MasterGroups WHERE MasterGroup=@masterGroup;";

                SqlCommand deleteMasterGroupQuery = new SqlCommand(queryString, connection);

                deleteMasterGroupQuery.Parameters.AddWithValue("@masterGroup", masterGroup);

                using (connection)
                {
                    connection.Open();
                    SqlDataReader reader = deleteMasterGroupQuery.ExecuteReader();
                    connection.Close();
                }
                return Json(new { success = true, message = "Success Deleting Master Group: " });
            }
            catch (Exception e)
            {
                _logger.LogError("Error Deleting Master Group: " + e);
                return Json(new { success = false, message = "Error Deleting Master Group: " + e });
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
    }
}
