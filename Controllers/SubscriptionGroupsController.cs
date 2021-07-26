using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eComm_Reporting_Application.Controllers
{
    public class SubscriptionGroupsController : Controller
    {
        public IActionResult Index()
        {
            List<string> groupsList = new List<string>();
            groupsList.Add("Ecomm SVP, Merchandising and Planning");
            groupsList.Add("Ecomm Merchandise Managers");
            groupsList.Add("Ecomm Admins");
            groupsList.Add("Ecomm Merch Admins");
            return View(groupsList);
        }
    }
}
