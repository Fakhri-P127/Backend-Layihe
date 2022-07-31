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
        public IActionResult Index()
        {
            List<Message> message = _context.Messages.ToList();

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
