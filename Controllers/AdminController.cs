using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eComm_Reporting_Application.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace eComm_Reporting_Application.Controllers
{
    public class AdminController : Controller
    {

        private readonly IConfiguration configuration;
        public AdminController(IConfiguration config)
        {
            this.configuration = config;
        }

        public IActionResult Index()
        {
            AdminPageModel adminModel = new AdminPageModel();

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
    }
}
