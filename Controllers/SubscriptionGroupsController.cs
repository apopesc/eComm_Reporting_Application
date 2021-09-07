﻿using Microsoft.AspNetCore.Mvc;
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

                SqlCommand groupsQuery = new SqlCommand("SELECT DISTINCT User_Group FROM UserSubscriptionFilters WHERE User_Group IS NOT NULL", connection);
                SqlCommand groupIDsQuery = new SqlCommand("SELECT DISTINCT Group_ID FROM UserSubscriptionFilters WHERE Group_ID IS NOT NULL", connection);
                SqlCommand masterGroupsQuery = new SqlCommand("SELECT DISTINCT Master_Group FROM UserSubscriptionFilters WHERE Master_Group IS NOT NULL", connection);

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
            catch(Exception e)
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

                //Converting lists to strings for the query
                string groupsListString = String.Join("', '", filterData.groupsList.ToArray());
                groupsListString = "'" + groupsListString + "'";
                string masterGroupsListString = String.Join("', '", filterData.masterGroupsList.ToArray());
                masterGroupsListString = "'" + masterGroupsListString + "'";

                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND User_Group IN (" + groupsListString + ") AND Master_Group IN (" + masterGroupsListString + ");", connection);

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


        public IActionResult AddNewUserSub()
        {
            UserSubscriptionDropdownModel subModel = GetFilterData();

            return View(subModel);
        }


        [HttpPost]
        public IActionResult AddUserSubToDB(string userEmail, string isActive, string selectedGroupID, string selectedGroup, string selectedMasterGroup)
        {
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                string addUserQueryString = "INSERT INTO UserSubscriptions (User_Email, Is_Active, User_Group, Group_ID, Master_Group) " +
                    "VALUES ('" + userEmail + "', '" + isActive + "', '" + selectedGroup + "', '" + selectedGroupID + "', '" + selectedMasterGroup + "');";

                SqlCommand addUserQuery = new SqlCommand(addUserQueryString, connection);

                string getUserIDString = "SELECT TOP 1* FROM UserSubscriptions ORDER BY ID Desc;"; //Getting the ID by getting the most recently added row

                SqlCommand getUserID = new SqlCommand(getUserIDString, connection);
                int userID = 0;

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
                newEntry.groupID = selectedGroupID;
                newEntry.group = selectedGroup;
                newEntry.masterGroup = selectedMasterGroup;

                tableData.Insert(0,newEntry); //Adding new user to start of table

                return Json(new {result = "Redirect", url = Url.Action("Index", "SubscriptionGroups")});
            }
            catch (Exception e)
            {
                return Json("Error Saving to Database: " + e);
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
                return Json("Error retrieving table data: " + e);
            }
        }
    }
}
