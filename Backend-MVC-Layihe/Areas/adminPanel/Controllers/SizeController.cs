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
    public class SizeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SizeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Size> sizes = _context.Sizes.ToList();
            return View(sizes);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Size size)
        {
            if (!ModelState.IsValid) return View();
            Size existed = _context.Sizes.FirstOrDefault(x =>
            x.Name.ToLower().Trim() == size.Name.ToLower().Trim());
            if (existed != null)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View();
            }
            _context.Sizes.Add(size);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Size existed = _context.Sizes.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int? id, Size newSize)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Size existed = _context.Sizes.FirstOrDefault(x => x.Id == id);
            if (existed is null) return BadRequest();

            bool isDuplicate = _context.Sizes.Any(x => x.Name.ToLower().Trim() == newSize.Name.ToLower().Trim());
            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View();
            }

            _context.Entry(existed).CurrentValues.SetValues(newSize);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (size is null) return NotFound();

            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Size existed = _context.Sizes.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
