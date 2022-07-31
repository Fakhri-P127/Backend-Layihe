using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminPanel")]
    [Authorize(Roles ="Moderator,Admin")]

    public class DashboardController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

    }
}
