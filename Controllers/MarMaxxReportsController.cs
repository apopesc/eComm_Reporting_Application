using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eComm_Reporting_Application.Models;

namespace eComm_Reporting_Application.Controllers
{
    public class MarMaxxReportsController : Controller
    {
        public IActionResult Index()
        {
            ReportPageDropdownModel marMaxxDropdownModel = new ReportPageDropdownModel();
            List<string> folder_list = new List<string>();
            
            folder_list.Add("Test folder 1");
            folder_list.Add("Test folder 2");
            folder_list.Add("Test folder 3");
            folder_list.Add("Test folder 4");

            marMaxxDropdownModel.folderList = folder_list;
            return View(marMaxxDropdownModel);
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
}
