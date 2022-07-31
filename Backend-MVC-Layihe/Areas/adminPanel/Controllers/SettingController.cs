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
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            byte ItemsPerPage = 6;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Settings.Count() / ItemsPerPage);

            List<Setting> settings = _context.Settings.OrderByDescending(c => c.Id)
                .Skip((page-1)*ItemsPerPage).Take(ItemsPerPage).ToList();
            return View(settings);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Setting setting)
        {
            if (!ModelState.IsValid) return View();
            Setting existed = await _context.Settings.FirstOrDefaultAsync(x =>
            x.Key.ToLower().Trim() == setting.Key.ToLower().Trim());

            if (existed != null)
            {
                ModelState.AddModelError("Name", "This setting already exists");
                return View();
            }
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Setting existed = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int? id, Setting newSetting)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Setting existed = _context.Settings.FirstOrDefault(x => x.Id == id);
            if (existed is null) return BadRequest();

            _context.Entry(existed).CurrentValues.SetValues(newSetting);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);

            if (setting is null) return NotFound();

            _context.Settings.Remove(setting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Setting existed = _context.Settings.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
