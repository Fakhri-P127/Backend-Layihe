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
    public class DiscountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            byte ItemsPerPage = 8;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Discounts.Count() / ItemsPerPage);

            List<Discount> discounts = _context.Discounts
                .OrderByDescending(c => c.Id).Skip((page-1)*ItemsPerPage)
                .Take(ItemsPerPage).ToList();
            return View(discounts);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Discount discount)
        {
            if (!ModelState.IsValid) return View();
            Discount existed = await _context.Discounts.FirstOrDefaultAsync(x =>
            x.PercentKey.ToLower().Trim() == discount.PercentKey.ToLower().Trim());

            if (existed != null)
            {
                ModelState.AddModelError("Name", "This discount already exists");
                return View();
            }
            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Discount existed = _context.Discounts.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(int? id, Discount newDiscount)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Discount existed = _context.Discounts.FirstOrDefault(x => x.Id == id);
            if (existed is null) return BadRequest();

           

            _context.Entry(existed).CurrentValues.SetValues(newDiscount);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Discount discount = await _context.Discounts.FirstOrDefaultAsync(x => x.Id == id);

            if (discount is null) return NotFound();

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Discount existed = _context.Discounts.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
