using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminPanel")]
    public class ColorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ColorController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Color> colors = _context.Colors.ToList();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Color color)
        {
            if (!ModelState.IsValid) return View();
            Color existed = _context.Colors.FirstOrDefault(x =>
            x.Name.ToLower().Trim() == color.Name.ToLower().Trim());
            if (existed != null)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View();
            }
            _context.Colors.Add(color);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color existed = _context.Colors.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int? id, Color newColor)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Color existed = _context.Colors.FirstOrDefault(x => x.Id == id);
            if (existed is null) return BadRequest();

            bool isDuplicate = _context.Colors.Any(x => x.Name.ToLower().Trim() == newColor.Name.ToLower().Trim());
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View();
            }

            _context.Entry(existed).CurrentValues.SetValues(newColor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (color is null) return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color existed = _context.Colors.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
