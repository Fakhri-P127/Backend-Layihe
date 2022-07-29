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
                .Include(c => c.ClothesImages)
                .Include(c => c.ClothesColors).ThenInclude(c => c.Color)
                .FirstOrDefault(c => c.Id == id && c.ClothesColors.Any(c=>c.ClothesId==id)),
                Clotheses = new List<Clothes>(),
                Category = _context.Clothes.Include(c => c.Category).FirstOrDefault(c => c.Id == id).Category,
                ClothesColorSizes = _context.ClothesColorSizes.Include(c => c.Size).Include(c => c.ClothesColor)
                .ThenInclude(c => c.Clothes).Where(s => s.ClothesColor.ClothesId == id).ToList()

            };
            List<Clothes> clothes = new List<Clothes>();

            foreach (Clothes product in model.Category.Clothes.ToList())
            {
                clothes = _context.Clothes.Include(x => x.Category).ThenInclude(c => c.Clothes)
                    .Include(x => x.ClothesImages)
                    .Where(p => product.CategoryId == p.CategoryId && p.Id !=id).ToList();
                    
                model.Clotheses.AddRange(clothes);
            }
            //model.Clotheses = model.Clotheses.Distinct().ToList();
            
            if (model.Clothes is null) return NotFound();

            //olan datalarimla muqasiye uchun
            ViewBag.Sizes = _context.Sizes.Include(s => s.ClothesColorSizes).ThenInclude(s => s.ClothesColor)
                .ThenInclude(s => s.Clothes).ToList();
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors)
                 .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToList();

            return View(model);
        }

        public async Task<IActionResult> GetDatas(int? clothesId, int? colorId)
        {
            Clothes existed = await _context.Clothes.FirstOrDefaultAsync(c => c.Id == clothesId);
            Color color = await _context.Colors.Include(c => c.ClothesColors)
                .FirstOrDefaultAsync(c => c.Id == colorId);
            if (color is null) return NotFound();
            ClothesColor clothesColor = await _context.ClothesColors
                .Include(c=>c.ClothesColorSizes).ThenInclude(c=>c.Size)
                .FirstOrDefaultAsync(c => c.ColorId == colorId && existed.Id == c.ClothesId);
            List<ClothesColorSize> clothesColorSize = await _context.ClothesColorSizes.Include(c => c.Size)
                .Where(c => c.ClothesColorId == clothesColor.Id).ToListAsync();

            return PartialView("_SizesFetchPartialView", clothesColorSize);
           
        }
    }
}
