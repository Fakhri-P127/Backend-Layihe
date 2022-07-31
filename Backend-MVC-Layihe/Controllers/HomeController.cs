using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Controllers
{
    public class HomeController:Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            HomeVM model = new HomeVM
            {
                Sliders = await _context.Sliders.ToListAsync(),
                Clothes= await _context.Clothes
                .Include(c => c.ClothesImages)
                .Include(c=>c.Category)
                .ToListAsync(),
                Categories = await _context.Categories.Include(c=>c.Clothes).ToListAsync(),
                SpecialOffers= await _context.SpecialOffers.ToListAsync()

            };
            
            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Contact(Message message)
        {
            if (!ModelState.IsValid) return View();

            if (User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                ModelState.AddModelError(string.Empty,"Only Member and guests are allowed to send emails.");
                return View();
            }
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["Successfull"] = "Your message has been send successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
