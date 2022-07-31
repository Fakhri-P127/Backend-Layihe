using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            byte ItemsPerPage = 8;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Clothes.Count() / ItemsPerPage);

            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ClothesColorSizes)
                .ThenInclude(c=>c.ClothesColor).ThenInclude(c=>c.Clothes).ToList();
            if(categoryId != null || categoryId != 0)
            {
                Category category = await _context.Categories
                .Include(c => c.Clothes).ThenInclude(c=>c.ClothesImages).FirstOrDefaultAsync(c => c.Id == categoryId);
                if(category != null)
                {
                    if(category.Clothes.Count() == 0)
                    {
                        ViewBag.Msg = "There's no products in this category";
                    }
                    return View(category.Clothes);
                  
                }
            }
            List<Clothes> clothes = _context.Clothes
                .Include(c => c.ClothesImages)
             .Include(c => c.ClothesColors).ThenInclude(c => c.Color).OrderByDescending(c => c.Id)
             .Skip((page - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
     
            return View(clothes);
        }
        public IActionResult GetDatas(string sortingOrder)
        {
            List<Clothes> clothes = _context.Clothes
             .Include(c => c.ClothesImages)
             .Include(c=>c.ClothesColors).ThenInclude(c=>c.Color)
             .OrderByDescending(c => c.Id).ToList();

            //sorting
            switch (sortingOrder)
            {
                case "A-Z":
                    clothes = clothes.OrderBy(clothes => clothes.Name).ToList();
                    break;
                case "Z-A":
                    clothes = clothes.OrderByDescending(clothes => clothes.Name).ToList();
                    break;
                case "Price by ascending":
                    clothes = clothes.OrderBy(clothes => clothes.Price).ToList();
                    break;
                case "Price by descending":
                    clothes = clothes.OrderByDescending(clothes => clothes.Price).ToList();
                    break;
                default:
                    clothes = clothes.OrderByDescending(clothes => clothes.Id).ToList();
                    break;
            }
        
            //sorting for colors
            List<Color> colors = _context.Colors.Include(c => c.ClothesColors).ToList();
            foreach (Color color in colors)
            {
               if(color.Name == sortingOrder)
                {
                    clothes = clothes.Where(c => c.ClothesColors.Any(d => d.Color.Name == sortingOrder)).ToList();
                    break;
                }
                else if(sortingOrder == "allColors")
                {
                    clothes = clothes.OrderByDescending(c=>c.Id).ToList();
                    break;
                }
            }     
            return PartialView("_ClothesPartialView", clothes.OrderByDescending(c=>c.Id).Take(8).ToList());
        }

       
    }
}
