using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eComm_Reporting_Application.Models;

namespace eComm_Reporting_Application.Controllers
{
    public class SubscriptionGroupsController : Controller
    {
        public IActionResult Index()
        {
            int is_active = 0;
            
            // ------------------- Temporarily Hard Coded Data, will be pulling this from DB in the future ------------------//
            List<string> groupsID_list = new List<string>();
            groupsID_list.Add("SVP");
            groupsID_list.Add("MM");
            groupsID_list.Add("Admin");
            groupsID_list.Add("NYBO MM Assist");
            
            List<string> groups_list = new List<string>();
            groups_list.Add("Ecomm SVP, Merchandising and Planning");
            groups_list.Add("Ecomm Merchandise Managers");
            groups_list.Add("Ecomm Admins");
            groups_list.Add("Ecomm Merch Admins");
            
            List<string> master_groups_list = new List<string>();
            master_groups_list.Add("Merchandising");
            master_groups_list.Add("Admin");
            master_groups_list.Add("Fulfillment");
            master_groups_list.Add("Ecomm IT");
            //--------------------------------------------------------------------------------------------------------------//

            SubscriptionGroupsModel subModel = new SubscriptionGroupsModel() {
                isActive = is_active,
                groupsIDList = groupsID_list,
                groupsList = groups_list,
                masterGroupsList = master_groups_list
            };
            
            return View(subModel);
        }

        [HttpPost]
        public IActionResult ReceiveFilters(int isActive, List<string> selectedGroupIDs, List<string> selectedGroups, List<string> selectedMasterGroups)
        {
            SubscriptionGroupsModel filterData = new SubscriptionGroupsModel();

            //Calling the function to get the table data
            List<SubscriptionGroupsTableModel> tableData = GetTableData(filterData);
            
            //Returning the table data to the front end
            return Json(tableData);
        }

        public List<SubscriptionGroupsTableModel> GetTableData(SubscriptionGroupsModel filterData)
        {   
            //Need to pass filterData to a DB stored procedure to get data for this list
            List<SubscriptionGroupsTableModel> tableData = new List<SubscriptionGroupsTableModel>();
            //It is a list of objects (each object property maps to a column - userEmail, isActive, etc...)

            // ------------------- Temporarily Hard Coded Data, will be pulling this from DB in the future ------------------//
            SubscriptionGroupsTableModel entry1 = new SubscriptionGroupsTableModel();
            entry1.userEmail = "andrei_popescu@tjxcanada.ca";
            entry1.isActive = 'Y';
            entry1.group = "Ecomm SVP, Merchandising and Planning";
            entry1.groupID = "SVP";
            entry1.masterGroup = "Merchandising";
            tableData.Add(entry1);

            SubscriptionGroupsTableModel entry2 = new SubscriptionGroupsTableModel();
            entry2.userEmail = "test_guy@tjxcanada.ca";
            entry2.isActive = 'Y';
            entry2.group = "Ecomm SVP, Merchandising and Planning";
            entry2.groupID = "SVP";
            entry2.masterGroup = "Merchandising";
            tableData.Add(entry2);

            SubscriptionGroupsTableModel entry3 = new SubscriptionGroupsTableModel();
            entry3.userEmail = "testing@tjxcanada.ca";
            entry3.isActive = 'Y';
            entry3.group = "Ecomm SVP, Merchandising and Planning";
            entry3.groupID = "SVP";
            entry3.masterGroup = "Merchandising";
            tableData.Add(entry3);

            SubscriptionGroupsTableModel entry4 = new SubscriptionGroupsTableModel();
            entry4.userEmail = "testdude@tjxcanada.ca";
            entry4.isActive = 'Y';
            entry4.group = "Ecomm SVP, Merchandising and Planning";
            entry4.groupID = "SVP";
            entry4.masterGroup = "Merchandising";
            tableData.Add(entry4);

            //--------------------------------------------------------------------------------------------------------------//
            return tableData;
        }
    }
}
