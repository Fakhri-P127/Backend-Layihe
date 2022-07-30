using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels.Cart;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;

        public OrderController(ApplicationDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            return View();
        }

        public async Task<IActionResult> Cart()
        {
            CartVM cart = new CartVM();
                cart.CartItemVMs = new List<CartItemVM>();
                cart.TotalPrice = 0;

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.BasketItems = await _context.BasketItems
                    .Include(b => b.AppUser).Include(b => b.Clothes)
                    .Where(b => b.AppUserId == user.Id).ToListAsync();

                foreach (CartItem basketItem in user.BasketItems.ToList())
                {
                    ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
                        .ThenInclude(c=>c.ClothesImages).Include(c => c.ClothesColorSizes)
             .FirstOrDefaultAsync(c => c.ClothesId == basketItem.ClothesId
             && c.ColorId == basketItem.ColorId
             && c.ClothesColorSizes.Any(c => c.SizeId == basketItem.SizeId));

                    if (clothesColor is null)
                    {
                        user.BasketItems.Remove(basketItem);
                        continue;
                    }
                    CartItemVM cartItem = new CartItemVM
                    {
                        Clothes = clothesColor.Clothes,
                        Quantity = basketItem.Quantity,
                        ColorId = basketItem.ColorId,
                        SizeId = basketItem.SizeId
                    };
                    cart.CartItemVMs.Add(cartItem);
                    cart.TotalPrice += clothesColor.Clothes.DiscountPrice * basketItem.Quantity;
                }

            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr))
                {
                    ViewBag.Empty = "No Products";
                    return View();
                }
                CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(cartCookieStr);
                if (cartCookie is null) return NotFound();

                foreach (CartCookieItemVM cookieItem in cartCookie.CartCookieItemVMs)
                {
                    Clothes clothes = await _context.Clothes.Include(c => c.ClothesImages).Include(c => c.ClothesColors)
                        .ThenInclude(c => c.Color).Include(c => c.ClothesColors).ThenInclude(c => c.ClothesColorSizes)
                        .ThenInclude(c => c.Size).FirstOrDefaultAsync(c => c.Id == cookieItem.Id);

                    CartItemVM cartItem = new CartItemVM
                    {
                        Clothes = clothes,
                        ColorId = cookieItem.ColorId,
                        SizeId = cookieItem.SizeId,
                        Quantity = cookieItem.Quantity
                    };
                    cart.CartItemVMs.Add(cartItem);
                    cart.TotalPrice += clothes.DiscountPrice * cookieItem.Quantity;
                }
            }
           

            ViewBag.Colors = await _context.Colors.ToListAsync();
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            return View(cart);
        }


        public async Task<IActionResult> RemoveItem(int? id,int? colorId,int? sizeId)
        {
            if (id is null || id == 0 || colorId is null || colorId == 0 ||
                sizeId is null || sizeId == 0) return NotFound();

            ClothesColor clothesColor;
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is null) return NotFound();
                user.BasketItems = await _context.BasketItems.Include(b=>b.AppUser).Include(b=>b.Clothes).ThenInclude(c=>c.ClothesColors).ToListAsync();
                CartItem cartItem = user.BasketItems.FirstOrDefault(b => b.ClothesId == id && b.ColorId == colorId
                && sizeId == b.SizeId);


                 clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
                  .Include(c => c.ClothesColorSizes)
                 .FirstOrDefaultAsync(c => c.ClothesId == cartItem.ClothesId
                 && c.ColorId == cartItem.ColorId
                 && c.ClothesColorSizes.Any(c => c.SizeId == cartItem.SizeId));


                user.BasketItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                //user. -= existedCookieItem.Quantity * clothesColor.Clothes.DiscountPrice;
                //CartVM cartVM = _context.

            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

                CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
                CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                       .FirstOrDefault(c => c.Id == id && c.ColorId == colorId && c.SizeId == sizeId);

                clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
                   .Include(c => c.ClothesColorSizes)
                  .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id
                  && c.ColorId == existedCookieItem.ColorId
                  && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));

                cartCookie.CartCookieItemVMs.Remove(existedCookieItem);
                cartCookie.TotalPrice -= existedCookieItem.Quantity * clothesColor.Clothes.DiscountPrice;
                cartCookieStr = JsonConvert.SerializeObject(cartCookie);
                HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            }
        
            TempData["Message"] = $"{clothesColor.Clothes.Name} product has been removed";

            return RedirectToAction(nameof(Cart));
        }
        public async Task<IActionResult> QuantityPlus(int? id,int? colorId,int? sizeId)
        {
            if (id is null || id == 0 || colorId is null || colorId == 0 ||
                sizeId is null || sizeId == 0) return NotFound();

            string cartCookieStr = HttpContext.Request.Cookies["Cart"];
            if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

            CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
            CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                .FirstOrDefault(c => c.Id == id && c.ColorId== colorId && c.SizeId == sizeId);

            if (existedCookieItem.Quantity >= 10)
            {
                TempData["Message"] = "The limit is 10 per product";
                return RedirectToAction(nameof(Cart));
            }
            ClothesColor clothesColor = await _context.ClothesColors.Include(c=>c.Clothes)
                .Include(c=>c.ClothesColorSizes)
               .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id 
               && c.ColorId == existedCookieItem.ColorId
               && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));
            
            existedCookieItem.Quantity++;
            cartCookie.TotalPrice += clothesColor.Clothes.DiscountPrice;

            cartCookieStr = JsonConvert.SerializeObject(cartCookie);
            HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            return RedirectToAction(nameof(Cart));
        }
        public async Task<IActionResult> QuantityMinus(int? id,int? colorId, int? sizeId)
        {
            if (id is null || id == 0 || colorId is null || colorId == 0 ||
                sizeId is null || sizeId == 0) return NotFound();

            string cartCookieStr = HttpContext.Request.Cookies["Cart"];
            if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

            CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
            CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                  .FirstOrDefault(c => c.Id == id && c.ColorId == colorId && c.SizeId == sizeId);

            ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
                .Include(c => c.ClothesColorSizes)
               .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id
               && c.ColorId == existedCookieItem.ColorId
               && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));

            //sayi sifir olursa basketden silsin
            if(existedCookieItem.Quantity > 1)
            {
                existedCookieItem.Quantity--;
                cartCookie.TotalPrice -= clothesColor.Clothes.DiscountPrice;
            }
            else
            {
                cartCookie.CartCookieItemVMs.Remove(existedCookieItem);
                cartCookie.TotalPrice -= existedCookieItem.Quantity * clothesColor.Clothes.DiscountPrice;
                TempData["Message"] = $"{clothesColor.Clothes.Name} product has been removed";
            }

            cartCookieStr = JsonConvert.SerializeObject(cartCookie);
            HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            return RedirectToAction(nameof(Cart));
        }
    }
}
