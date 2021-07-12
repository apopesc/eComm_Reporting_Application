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
            return View();
        }
    }
}
