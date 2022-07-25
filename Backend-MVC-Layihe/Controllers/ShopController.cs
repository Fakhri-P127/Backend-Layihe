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
            List<Clothes> clothes = _context.Clothes.Include(c => c.ClothesImages).OrderByDescending(c => c.Id).Take(9).ToList();
            //TempData["ClothesOrder"] = _context.Clothes.Include(c => c.ClothesImages).OrderByDescending(c => c.Id).Take(9).ToList();
            return View(clothes);
        }

        public  IActionResult GetLatest()
        {
            //string basketStr = JsonConvert.SerializeObject(_context.Clothes.Include(c => c.ClothesImages).OrderByDescending(c => c.Id).Take(9).ToList());
            //TempData["ClothesOrder"] = basketStr;
            TempData["ClothesOrder"] = _context.Clothes.Include(c => c.ClothesImages).OrderByDescending(c => c.Id).Take(9).ToList();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> GetAlphabeticOrder()
        {
            TempData["ClothesOrder"] = await _context.Clothes.Include(c => c.ClothesImages).OrderBy(c => c.Name).Take(9).ToListAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetReverseAlphabeticOrder()
        {
            TempData["ClothesOrder"] = await _context.Clothes.Include(c => c.ClothesImages).OrderByDescending(c => c.Name).Take(9).ToListAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
