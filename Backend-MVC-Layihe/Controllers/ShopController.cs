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
        public IActionResult Index()
        {
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ColorSizes).ToList();
            List<Clothes> clothes = _context.Clothes
                .Include(c => c.ClothesImages)
             .Include(c => c.ClothesColors).ThenInclude(c => c.Color).OrderByDescending(c => c.Id).ToList();
     
            return View(clothes);
        }
        public IActionResult GetDatas(string sortingOrder)
        {
            List<Clothes> clothes = _context.Clothes
             .Include(c => c.ClothesImages)
             .Include(c=>c.ClothesColors).ThenInclude(c=>c.Color)
             .OrderByDescending(c => c.Id).ToList();
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
           
            return PartialView("_ClothesFetchPartialView", clothes);
        }

       
    }
}
