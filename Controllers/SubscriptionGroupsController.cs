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
            bool is_active = false;
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
            List<string> master_groupsID_list = new List<string>();
            master_groupsID_list.Add("Merchandising");
            master_groupsID_list.Add("Admin");
            master_groupsID_list.Add("Fulfillment");
            master_groupsID_list.Add("Ecomm IT");

            SubscriptionGroupsModel subModel = new SubscriptionGroupsModel() {
                isActive = is_active,
                groupsIDList = groupsID_list,
                groupsList = groups_list,
                masterGroupsIDList = master_groupsID_list
            };
            
            return View(subModel);
        }
    }
}
