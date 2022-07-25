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
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ProductVM model = new ProductVM
            {
                Clothes = _context.Clothes
                .Include(c => c.ClothesImages).Include(c => c.ClothesCategories).ThenInclude(c => c.Category)
                .Include(c => c.ClothesColors).ThenInclude(c => c.Color)
                .ThenInclude(c => c.ColorSizes).ThenInclude(c => c.Size)
                .FirstOrDefault(c => c.Id == id),
                Clotheses = new List<Clothes>()
            };
            List<Clothes> clothes = new List<Clothes>();

            foreach (ClothesCategory category in model.Clothes.ClothesCategories)
            {

                clothes =  _context.Clothes.Include(x => x.ClothesCategories)
                    .Include(x => x.ClothesImages)
                    .Where(p => p.ClothesCategories
                    .Any(x => x.CategoryId == category.CategoryId) && p.Id != category.ClothesId).ToList();

                model.Clotheses.AddRange(clothes);
            }
            model.Clotheses = model.Clotheses.Distinct().ToList();
            
            if (model.Clothes is null) return NotFound();

            return View(model);
        }

    }
}
