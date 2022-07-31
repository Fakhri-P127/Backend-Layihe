using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminPanel")]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            byte ItemsPerPage = 4;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Messages.Count() / ItemsPerPage);

            List<Message> message = _context.Messages
               .OrderByDescending(c => c.Id).Skip((page-1)*ItemsPerPage)
               .Take(ItemsPerPage).ToList();

            return View(message);
        }
        public IActionResult Details(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Message message = _context.Messages.FirstOrDefault(c => c.Id == id);
            return View(message);
        }
    }
}
