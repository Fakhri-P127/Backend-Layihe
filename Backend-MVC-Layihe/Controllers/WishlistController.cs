using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Controllers
{
    [Authorize(Roles ="Member")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishlistController(ApplicationDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            byte ItemsPerPage = 4;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Clothes.Count() / ItemsPerPage);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return NotFound();
            user.WishlistItems = await _context.WishlistItems.Include(w => w.Clothes).ThenInclude(c => c.ClothesImages)
                .Include(w => w.AppUser).Where(w => w.AppUserId == user.Id).ToListAsync();
            if (user.WishlistItems.Count == 0)
            {
                TempData["Empty"] = "There's no item in your wishlist.";
                return View();
            }
            return View(user.WishlistItems.Skip((page-1)*ItemsPerPage).Take(ItemsPerPage).ToList());
        }

        public async Task<IActionResult> AddWishlist(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Clothes existed = await _context.Clothes.Include(c => c.ClothesImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return NotFound();
            user.WishlistItems = await _context.WishlistItems.Include(w => w.Clothes).ThenInclude(c=>c.ClothesImages)
                .Include(w => w.AppUser).Where(w => w.AppUserId == user.Id).ToListAsync();

            if (user.WishlistItems.Any(w => w.ClothesId == id))
            {
                TempData["Empty"] = "This product is already in wishlist.";
                return RedirectToAction(nameof(Index));
            }
            WishlistItem wishlistItem = new WishlistItem
            {
                AppUser = user,
                Clothes = existed
            };
            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RemoveWishlist(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Clothes existed = await _context.Clothes.Include(c => c.ClothesImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return NotFound();
            user.WishlistItems = await _context.WishlistItems.Include(w => w.Clothes)
                .Include(w => w.AppUser).Where(w => w.AppUserId == user.Id).ToListAsync();

            if (!user.WishlistItems.Any(w => w.ClothesId == id)) return NotFound();

            WishlistItem wishlistItem = await _context.WishlistItems.Include(c => c.Clothes)
                .FirstOrDefaultAsync(w => w.ClothesId == id && w.AppUserId == user.Id);
            _context.WishlistItems.Remove(wishlistItem);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
