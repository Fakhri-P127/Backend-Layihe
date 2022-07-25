using Backend_MVC_Layihe.DAL;
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
                .Include(c=>c.ClothesCategories).ThenInclude(c=>c.Category)
                .Include(c=>c.ClothesImages)
                .ToListAsync(),
                SpecialOffers= await _context.SpecialOffers.ToListAsync()
            };
            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
