using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace eComm_Reporting_Application.Controllers
{
    public class SubscriptionGroupsController : Controller
    {

        private readonly IConfiguration configuration;
        public static List<SubscriptionGroupsTableModel> tableData = new List<SubscriptionGroupsTableModel>();


        public SubscriptionGroupsController(IConfiguration config)
        {
            this.configuration = config;
        }


        public IActionResult Index()
        {
            SubscriptionGroupsModel subModel = GetFilterData(); 
            
            return View(subModel);
        }


        //Getting filter data from the database
        private SubscriptionGroupsModel GetFilterData()
        {
            int is_active = 0;

            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

            SqlConnection connection = new SqlConnection(connectionstring);

            SqlCommand groupsQuery = new SqlCommand("SELECT DISTINCT User_Group FROM UserSubscriptionFilters WHERE User_Group IS NOT NULL", connection);
            SqlCommand groupIDsQuery = new SqlCommand("SELECT DISTINCT Group_ID FROM UserSubscriptionFilters WHERE Group_ID IS NOT NULL", connection);
            SqlCommand masterGroupsQuery = new SqlCommand("SELECT DISTINCT Master_Group FROM UserSubscriptionFilters WHERE Master_Group IS NOT NULL", connection);

            List<string> groups_list = new List<string>();
            List<string> groupsID_list = new List<string>();
            List<string> master_groups_list = new List<string>();

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

            SubscriptionGroupsModel filterDataModel = new SubscriptionGroupsModel()
            {
                isActive = is_active,
                groupsIDList = groupsID_list,
                groupsList = groups_list,
                masterGroupsList = master_groups_list
            };

            return filterDataModel;
        }


        [HttpPost]
        public IActionResult ReceiveFilters(int isActive, List<string> selectedGroups, List<string> selectedMasterGroups)
        {
            try
            {
                SubscriptionGroupsModel filterData = new SubscriptionGroupsModel();
                filterData.isActive = isActive;
                filterData.groupsList = selectedGroups;
                filterData.masterGroupsList = selectedMasterGroups;

                //Calling the function to get the table data
                tableData = GetTableData(filterData);

                //Returning the table data to the front end
                return Json(tableData);
            }
            catch (Exception e)
            {
                return Json("Error retrieving table data: " + e);
            }
            
        }


        private List<SubscriptionGroupsTableModel> GetTableData(SubscriptionGroupsModel filterData)
        {
            //A list of objects (each object property maps to a column - userEmail, isActive, etc...)
            List<SubscriptionGroupsTableModel> dbTableData = new List<SubscriptionGroupsTableModel>();
            
            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlCommand tableQuery;

            //Setting the isActive value for the query
            string isActiveString;
            if(filterData.isActive == 1)
            {
                isActiveString = "'Y'";
            }
            else if(filterData.isActive == 2)
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
                        SubscriptionGroupsTableModel entry = new SubscriptionGroupsTableModel();
                        entry.ID = reader.GetInt32(0);
                        entry.userEmail = reader.GetString(1);
                        entry.isActive = reader.GetString(2);
                        entry.group = reader.GetString(3);
                        entry.groupID = reader.GetString(4);
                        entry.masterGroup = reader.GetString(5);
                        dbTableData.Add(entry);
                    }
                }
                connection.Close();
            }

            return dbTableData;
        }


        public IActionResult AddNewUser()
        {
            SubscriptionGroupsModel subModel = GetFilterData();

            return View(subModel);
        }


        [HttpPost]
        public IActionResult AddUserToDB(string userEmail, string isActive, string selectedGroupID, string selectedGroup, string selectedMasterGroup)
        {
            try
            {
                string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");

                SqlConnection connection = new SqlConnection(connectionstring);

                SqlCommand addUserQuery = new SqlCommand("INSERT INTO UserSubscriptions (User_Email, Is_Active, User_Group, Group_ID, Master_Group) " +
                    "VALUES ('" + userEmail + "', '" + isActive + "', '" + selectedGroup + "', '" + selectedGroupID + "', '" + selectedMasterGroup + "');", connection);

                SqlCommand getUserID = new SqlCommand("SELECT TOP 1* FROM UserSubscriptions ORDER BY ID Desc;",connection); //Getting the ID by getting the most recently added row
                int userID = 0;

                using (connection)
                {
                    connection.Open();
                    SqlDataReader reader = addUserQuery.ExecuteReader();
                    connection.Close();

                    connection.Open();
                    SqlDataReader reader_ID = getUserID.ExecuteReader();
                    while (reader_ID.Read())
                    {
                        var id = reader_ID.GetInt32(0);
                        userID = id;
                    }
                    connection.Close();
                }

                SubscriptionGroupsTableModel newEntry = new SubscriptionGroupsTableModel();
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
        public IActionResult GetInitialTable()
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
    }
}
