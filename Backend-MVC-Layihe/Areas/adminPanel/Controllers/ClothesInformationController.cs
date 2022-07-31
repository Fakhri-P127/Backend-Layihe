using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminPanel")]
    [Authorize(Roles = "Moderator,Admin")]
    public class ClothesInformationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClothesInformationController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            byte ItemsPerPage = 6;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.ClothesInformations.Count() / ItemsPerPage);

            List<ClothesInformation> clothesInformation = _context.ClothesInformations
                .OrderByDescending(c => c.Id).Skip((page-1)*ItemsPerPage)
                .Take(ItemsPerPage).ToList();
            return View(clothesInformation);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ClothesInformation clothesInformation)
        {
            if (!ModelState.IsValid) return View();
            ClothesInformation existed = await _context.ClothesInformations.FirstOrDefaultAsync(x =>
            x.Definition.ToLower().Trim() == clothesInformation.Definition.ToLower().Trim());

            if (existed != null)
            {
                ModelState.AddModelError("Name", "This info already exists");
                return View();
            }
            await _context.ClothesInformations.AddAsync(clothesInformation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ClothesInformation existed = _context.ClothesInformations.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int? id, ClothesInformation newClothesInformation)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            ClothesInformation existed = _context.ClothesInformations.FirstOrDefault(x => x.Id == id);
            if (existed is null) return BadRequest();


            _context.Entry(existed).CurrentValues.SetValues(newClothesInformation);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ClothesInformation clothesInformation = await _context.ClothesInformations.FirstOrDefaultAsync(x => x.Id == id);

            if (clothesInformation is null) return NotFound();

            _context.ClothesInformations.Remove(clothesInformation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ClothesInformation existed = _context.ClothesInformations.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
