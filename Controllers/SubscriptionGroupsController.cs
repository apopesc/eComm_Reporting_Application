using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace eComm_Reporting_Application.Controllers
{
    public class SubscriptionGroupsController : Controller
    {

        private readonly IConfiguration configuration;
        public static List<UserSubscriptionTableModel> tableData = new List<UserSubscriptionTableModel>();


        public SubscriptionGroupsController(IConfiguration config)
        {
            this.configuration = config;
        }


        public IActionResult Index()
        {
            UserSubscriptionDropdownModel subModel = GetFilterData(); 
            
            return View(subModel);
        }

        public IActionResult AddNewUserSub()
        {
            UserSubscriptionDropdownModel subModel = GetFilterData();

            return View(subModel);
        }

        public IActionResult EditUserSub(int ID)
        {
            EditUserDropdownModel subModel = new EditUserDropdownModel();

            List<string> master_groups_list = new List<string>();

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);

            string masterGroupsQueryString = "SELECT DISTINCT MasterGroup FROM Groups WHERE MasterGroup IS NOT NULL";
            string userQueryString = "SELECT * FROM UserSubscriptions WHERE ID=" + ID;

            SqlCommand masterGroupsQuery = new SqlCommand(masterGroupsQueryString, connection);
            SqlCommand userQuery = new SqlCommand(userQueryString, connection);

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
                        subModel.selectedMasterGroups = reader.GetString(4);
                    }
                }
                connection.Close();
            }

            subModel.ID = ID;
            subModel.masterGroupsList = master_groups_list;

            return View(subModel);
        }

        [HttpPost]
        public IActionResult AddUserSubToDB(string userEmail, string isActive, string selectedGroupIDs, string selectedGroups, string selectedMasterGroups)
        {
            try
            {
                int userID = 0;

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                string addUserQueryString = "INSERT INTO UserSubscriptions (User_Email, Is_Active, User_Group, Group_ID, Master_Group) " +
                    "VALUES ('" + userEmail + "', '" + isActive + "', '" + selectedGroups + "', '" + selectedGroupIDs + "', '" + selectedMasterGroups + "');";

                SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);

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

                tableData.Insert(0, newEntry); //Adding new user to start of table

                return Json(new { result = "Redirect", url = Url.Action("Index", "SubscriptionGroups") });
            }
            catch (Exception e)
            {
                return Json("Error Saving to Database: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetTableData(UserSubscriptionDropdownModel filterData)
        {
            try
            {
                tableData = new List<UserSubscriptionTableModel>();//Resetting the table each time we want to get new data

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);
                SqlCommand tableQuery;

                //Setting the isActive value for the query
                string isActiveString;
                if (filterData.isActive == 1)
                {
                    isActiveString = "'Y'";
                }
                else if (filterData.isActive == 2)
                {
                    isActiveString = "'N'";
                }
                else //If both are selected, show all data for isActive
                {
                    isActiveString = "'Y', 'N'";
                }

                string queryString = "SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND (User_Group LIKE ";

                for(int i = 0; i<filterData.groupsList.Count; i++)
                {
                    if (i == 0)
                    {
                        queryString = queryString + "'%"+filterData.groupsList[i]+"%'";
                    }
                    else
                    {
                        queryString = queryString + " OR User_Group LIKE " + "'%" + filterData.groupsList[i] + "%'";
                    }
                }

                queryString = queryString + ") AND (Group_ID LIKE ";

                for (int i = 0; i < filterData.groupsIDList.Count; i++)
                {
                    if (i == 0)
                    {
                        queryString = queryString + "'%" + filterData.groupsIDList[i] + "%'";
                    }
                    else
                    {
                        queryString = queryString + " OR Group_ID LIKE " + "'%" + filterData.groupsIDList[i] + "%'";
                    }
                }

                queryString = queryString + ") AND (Master_Group LIKE ";

                for (int i = 0; i < filterData.masterGroupsList.Count; i++)
                {
                    if (i == 0)
                    {
                        queryString = queryString + "'%" + filterData.masterGroupsList[i] + "%'";
                    }
                    else
                    {
                        queryString = queryString + " OR Master_Group LIKE " + "'%" + filterData.masterGroupsList[i] + "%'";
                    }
                }

                queryString = queryString + ");";

                tableQuery = new SqlCommand(queryString, connection);
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
                            tableData.Add(entry);
                        }
                    }
                    connection.Close();
                }

                //Returning the table data to the front end
                return Json(tableData);
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
                return Json(tableData);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
        }

        [HttpPost]
        public JsonResult EditUserSub(List<UserSubscriptionTableModel> editedUsersList)
        {
            try
            {
                string successString = "Success editing users: ";

                for (int i = 0; i < editedUsersList.Count; i++)
                {
                    string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                    SqlConnection connection = new SqlConnection(connectionstring);

                    string queryString = "Update UserSubscriptions SET User_Email='" + editedUsersList[i].userEmail + "', Is_Active='" + editedUsersList[i].isActive + "', User_Group='" + editedUsersList[i].group +
                        "', Group_ID='" + editedUsersList[i].groupID + "', Master_Group='" + editedUsersList[i].masterGroup + "' WHERE ID=" + editedUsersList[i].ID + ";";

                    SqlCommand editUserQuery = new SqlCommand(queryString, connection);
                    using (connection)
                    {
                        connection.Open();
                        SqlDataReader reader = editUserQuery.ExecuteReader();
                        connection.Close();
                    }
                    successString = successString + editedUsersList[i].userEmail + ", ";
                }

                successString = successString.Substring(0, successString.Length - 2);
                return Json(successString);
            }
            catch (Exception e)
            {
                return Json("Error editing user: " + e);
            }
        }

        [HttpPost]
        public JsonResult DeleteUserSub(int ID)
        {
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string queryString = "DELETE FROM UserSubscriptions WHERE ID="+ID;

                SqlCommand deleteUserQuery = new SqlCommand(queryString, connection);
                using (connection)
                {
                    connection.Open();
                    SqlDataReader reader = deleteUserQuery.ExecuteReader();
                    connection.Close();
                }
                tableData.RemoveAll(x => x.ID == ID);

                return Json("Success Deleting User: " );
            }
            catch (Exception e)
            {
                return Json("Error deleting user: " + e);
            }
        }

        [HttpPost]
        public JsonResult GetGroupValues(List<string> masterGroupList)
        {
            try
            {
                IDictionary<string, string> groups = new Dictionary<string, string>();

                string masterGroupsListString = String.Join("', '", masterGroupList.ToArray());
                masterGroupsListString = "'" + masterGroupsListString + "'";

                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
                SqlConnection connection = new SqlConnection(connectionstring);

                string groupsQueryString = "SELECT GroupID,GroupName FROM Groups WHERE MasterGroup IN (" + masterGroupsListString + ")";
                SqlCommand groupsQuery = new SqlCommand(groupsQueryString, connection);
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
                return Json(groups);
            }
            catch (Exception e)
            {
                return Json("Error getting group values");
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

                string groupsQueryString = "SELECT DISTINCT GroupName FROM Groups WHERE GroupName IS NOT NULL";
                string groupIDsQueryString = "SELECT DISTINCT GroupID FROM Groups WHERE GroupID IS NOT NULL";
                string masterGroupsQueryString = "SELECT DISTINCT MasterGroup FROM Groups WHERE MasterGroup IS NOT NULL";

                SqlCommand groupsQuery = new SqlCommand(groupsQueryString, connection);
                SqlCommand groupIDsQuery = new SqlCommand(groupIDsQueryString, connection);
                SqlCommand masterGroupsQuery = new SqlCommand(masterGroupsQueryString, connection);

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
                    using (SqlDataReader reader = masterGroupsQuery.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var masterGroupString = reader.GetString(0);
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
