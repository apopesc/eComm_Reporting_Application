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
        public IActionResult ReceiveFilters(int isActive, List<string> selectedGroupIDs, List<string> selectedGroups, List<string> selectedMasterGroups)
        {
            SubscriptionGroupsModel filterData = new SubscriptionGroupsModel();
            filterData.isActive = isActive;
            filterData.groupsIDList = selectedGroupIDs;
            filterData.groupsList = selectedGroups;
            filterData.masterGroupsList = selectedMasterGroups;

            //Calling the function to get the table data
            List<SubscriptionGroupsTableModel> tableData = GetTableData(filterData);
            
            //Returning the table data to the front end
            return Json(tableData);
        }


        private List<SubscriptionGroupsTableModel> GetTableData(SubscriptionGroupsModel filterData)
        {
            //A list of objects (each object property maps to a column - userEmail, isActive, etc...)
            List<SubscriptionGroupsTableModel> tableData = new List<SubscriptionGroupsTableModel>();
            
            string connectionstring = configuration.GetConnectionString("ReportSubscriptions_DB");
            SqlConnection connection = new SqlConnection(connectionstring);
            SqlCommand tableQuery;

            //If dropdowns are empty, it is passing back a list with null as the first value, correcting that to an empty list.
            if(filterData.groupsList[0] == null)
            {
                filterData.groupsList.Clear();
            }
            if (filterData.groupsIDList[0] == null)
            {
                filterData.groupsIDList.Clear();
            }
            if (filterData.masterGroupsList[0] == null)
            {
                filterData.masterGroupsList.Clear();
            }

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
            else //If neither or both are selected, show all data for isActive
            {
                isActiveString = "'Y', 'N'";
            }

            //Converting lists to strings for the query
            string groupsListString = String.Join("', '", filterData.groupsList.ToArray());
            groupsListString = "'" + groupsListString + "'";
            string groupsIDListString = String.Join("', '", filterData.groupsIDList.ToArray());
            groupsIDListString = "'" + groupsIDListString + "'";
            string masterGroupsListString = String.Join("', '", filterData.masterGroupsList.ToArray());
            masterGroupsListString = "'" + masterGroupsListString + "'";

            //If none of the dropdowns are empty --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            if (filterData.groupsList.Count > 0 && filterData.groupsIDList.Count > 0 && filterData.masterGroupsList.Count > 0) 
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND User_Group IN (" + groupsListString + ") AND Group_ID IN (" + groupsIDListString + ") AND Master_Group IN (" + masterGroupsListString + ");", connection);
            }
            //If group dropdown is empty ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count == 0 && filterData.groupsIDList.Count > 0 && filterData.masterGroupsList.Count > 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND Group_ID IN (" + groupsIDListString + ") AND Master_Group IN (" + masterGroupsListString + ");", connection);
            }
            //If groupID dropdown is empty --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count > 0 && filterData.groupsIDList.Count == 0 && filterData.masterGroupsList.Count > 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND User_Group IN (" + groupsListString + ") AND Master_Group IN (" + masterGroupsListString + ");", connection);
            }
            //If master group dropdown is empty ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count > 0 && filterData.groupsIDList.Count > 0 && filterData.masterGroupsList.Count == 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND User_Group IN (" + groupsListString + ") AND Group_ID IN (" + groupsIDListString + ");", connection);
            }
            //If group and groupID dropdowns are empty --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count == 0 && filterData.groupsIDList.Count == 0 && filterData.masterGroupsList.Count > 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND Master_Group IN (" + masterGroupsListString + ");", connection);
            }
            //If group and masterGroup dropdowns are empty ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count == 0 && filterData.groupsIDList.Count > 0 && filterData.masterGroupsList.Count == 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND Group_ID IN (" + groupsIDListString + ");", connection);
            }
            //If groupID and masterGroup dropdowns are empty --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else if (filterData.groupsList.Count > 0 && filterData.groupsIDList.Count == 0 && filterData.masterGroupsList.Count == 0)
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ") AND User_Group IN (" + groupsListString + ");", connection);
            }
            //If all dropdowns are empty ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            else
            {
                tableQuery = new SqlCommand("SELECT * FROM UserSubscriptions WHERE Is_Active IN (" + isActiveString + ");", connection);
            }

            using (connection)
            {
                connection.Open();
                using (SqlDataReader reader = tableQuery.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SubscriptionGroupsTableModel entry = new SubscriptionGroupsTableModel();
                        entry.userEmail = reader.GetString(0);
                        entry.isActive = reader.GetString(1);
                        entry.group = reader.GetString(2);
                        entry.groupID = reader.GetString(3);
                        entry.masterGroup = reader.GetString(4);
                        tableData.Add(entry);
                    }
                }
                connection.Close();
            }

            return tableData;
        }


        public IActionResult AddNewUser()
        {
            SubscriptionGroupsModel subModel = GetFilterData();

            return View(subModel);
        }
    }
}
