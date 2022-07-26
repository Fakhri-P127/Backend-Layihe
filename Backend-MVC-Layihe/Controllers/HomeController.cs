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

        public IActionResult CategoryPage(int? id)
        {
            if (id is null || id == 0) return NotFound();

            List<Clothes> clothes = _context.Clothes.Include(c => c.ClothesImages)
                .Where(c => c.CategoryId == id).ToList();

            return View(clothes);
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
