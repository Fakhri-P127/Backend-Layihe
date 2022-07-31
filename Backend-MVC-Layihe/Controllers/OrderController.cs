using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout()
        {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null) return NotFound();
                user.CartItems = await _context.CartItems
                    .Include(c => c.Clothes).Include(c => c.AppUser)
                    .Where(c => c.AppUserId == user.Id).ToListAsync();
            if(user.CartItems.Count == 0)
            {
                TempData["Message"] = "Add some products to the cart first.";
                return RedirectToAction(nameof(Cart));
            }
                Order order = new Order
                {
                    CartItems = user.CartItems,
                    AppUser = user,
                    TotalPrice = default
                };
                user.CartItems.ForEach(item => order.TotalPrice += item.Price * item.Quantity);
            
            return View(order);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.CartItems = await _context.CartItems.Include(c => c.Clothes).Include(c => c.AppUser)
                .Where(c => c.AppUserId == user.Id).ToListAsync();
            if (user.CartItems.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "There's no items to buy");
                return View(order);
            }
            order.CartItems = user.CartItems;
            order.AppUser = user;
            order.Date = DateTime.Now;
            order.Status = null;
            order.TotalPrice = default;
            user.CartItems.ForEach(item => order.TotalPrice += item.Price * item.Quantity);
            if (!ModelState.IsValid)
            {
                return View(order);
            }
            await _context.Orders.AddAsync(order);
            user.CartItems = new List<CartItem>();
            await _context.SaveChangesAsync();

            TempData["OrderSucc"] = "Purchase has been successfull";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Cart()
        {
            CartVM cart = new CartVM();
                cart.CartItemVMs = new List<CartItemVM>();
                cart.TotalPrice = 0;

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                user.CartItems = await _context.CartItems
                    .Include(b => b.AppUser).Include(b => b.Clothes)
                    .Where(b => b.AppUserId == user.Id ).ToListAsync();

                if(user.CartItems.Count == 0)
                {
                        ViewBag.Empty = "No Products";
                        return View();
                }
                foreach (CartItem currCartItem in user.CartItems.ToList())
                {
                    ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
                        .ThenInclude(c=>c.ClothesImages).Include(c => c.ClothesColorSizes)
             .FirstOrDefaultAsync(c => c.ClothesId == currCartItem.ClothesId
             && c.ColorId == currCartItem.ColorId
             && c.ClothesColorSizes.Any(c => c.SizeId == currCartItem.SizeId));

                    if (clothesColor is null)
                    {
                        user.CartItems.Remove(currCartItem);
                        continue;
                    }
                    CartItemVM cartItem = new CartItemVM
                    {
                        Clothes = clothesColor.Clothes,
                        Quantity = currCartItem.Quantity,
                        ColorId = currCartItem.ColorId,
                        SizeId = currCartItem.SizeId
                    };
                  
                    cart.CartItemVMs.Add(cartItem);
                    cart.TotalPrice += clothesColor.Clothes.DiscountPrice * currCartItem.Quantity;
                }

            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                // Login olan shexs moderator ya da admindise o da bu if shertine dushecek
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

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is null) return NotFound();
                user.CartItems = await _context.CartItems.Include(b=>b.AppUser)
                    .Include(b=>b.Clothes).ThenInclude(c=>c.ClothesColors)
                    .Where(c=>c.AppUserId==user.Id).ToListAsync();

                CartItem cartItem = user.CartItems.FirstOrDefault(b => b.ClothesId == id && b.ColorId == colorId
                && sizeId == b.SizeId && user.Id == b.AppUserId);

                ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Color).Include(c => c.Clothes)
                  .Include(c => c.ClothesColorSizes).ThenInclude(c => c.Size)
                 .FirstOrDefaultAsync(c => c.ClothesId == cartItem.ClothesId
                 && c.ColorId == cartItem.ColorId
                 && c.ClothesColorSizes.Any(c => c.SizeId == cartItem.SizeId));

                string sizeName = clothesColor.ClothesColorSizes.FirstOrDefault(c => c.SizeId == cartItem.SizeId).Size.Name;

                TempData["Message"] = $"Product name:{clothesColor.Clothes.Name}/   Color:{clothesColor.Color.Name}/  Size:{sizeName}  has been removed";

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                
               
            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

                CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
                CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                       .FirstOrDefault(c => c.Id == id && c.ColorId == colorId && c.SizeId == sizeId);


                ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Color).Include(c => c.Clothes)
                  .Include(c => c.ClothesColorSizes).ThenInclude(c => c.Size)
                 .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id
                 && c.ColorId == existedCookieItem.ColorId
                 && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));

                cartCookie.CartCookieItemVMs.Remove(existedCookieItem);
                cartCookie.TotalPrice -= existedCookieItem.Quantity * clothesColor.Clothes.DiscountPrice;
                cartCookieStr = JsonConvert.SerializeObject(cartCookie);
                HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
                string sizeName = clothesColor.ClothesColorSizes.FirstOrDefault(c => c.SizeId == existedCookieItem.SizeId).Size.Name;
                
                TempData["Message"] = $"Product name:{clothesColor.Clothes.Name}/  Color:{clothesColor.Color.Name}/  Size:{sizeName}  has been removed";

            }


            return RedirectToAction(nameof(Cart));
        }
        public async Task<IActionResult> QuantityPlus(int? id,int? colorId,int? sizeId)
        {
            if (id is null || id == 0 || colorId is null || colorId == 0 ||
                sizeId is null || sizeId == 0) return NotFound();


            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is null) return NotFound();
                user.CartItems = await _context.CartItems.Include(b => b.AppUser)
                    .Include(b => b.Clothes).ThenInclude(c => c.ClothesColors)
                    .Where(c => c.AppUserId == user.Id).ToListAsync();

                CartItem cartItem = user.CartItems.FirstOrDefault(b => b.ClothesId == id && b.ColorId == colorId
                && sizeId == b.SizeId && user.Id == b.AppUserId);

                if (cartItem.Quantity >= 10)
                {
                    TempData["Message"] = "The limit is 10 per product";
                    return RedirectToAction(nameof(Cart));
                }
                cartItem.Quantity++;
                await _context.SaveChangesAsync();

            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

                CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
                CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                    .FirstOrDefault(c => c.Id == id && c.ColorId == colorId && c.SizeId == sizeId);

                if (existedCookieItem.Quantity >= 10)
                {
                    TempData["Message"] = "The limit is 10 per product";
                    return RedirectToAction(nameof(Cart));
                }
                ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Color).Include(c => c.Clothes)
                    .Include(c => c.ClothesColorSizes).ThenInclude(c => c.Size)
                   .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id
                   && c.ColorId == existedCookieItem.ColorId
                   && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));

                existedCookieItem.Quantity++;
                cartCookie.TotalPrice += clothesColor.Clothes.DiscountPrice;

                cartCookieStr = JsonConvert.SerializeObject(cartCookie);
                HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            }

            return RedirectToAction(nameof(Cart));
        }
        public async Task<IActionResult> QuantityMinus(int? id,int? colorId, int? sizeId)
        {
            if (id is null || id == 0 || colorId is null || colorId == 0 ||
                sizeId is null || sizeId == 0) return NotFound();

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is null) return NotFound();
                user.CartItems = await _context.CartItems.Include(b => b.AppUser)
                    .Include(b => b.Clothes).ThenInclude(c => c.ClothesColors).Include(c=>c.Clothes)
                    .ThenInclude(c=>c.ClothesImages)
                    .Where(c => c.AppUserId == user.Id).ToListAsync();

                CartItem cartItem = user.CartItems.FirstOrDefault(b => b.ClothesId == id && b.ColorId == colorId
                && sizeId == b.SizeId && user.Id == b.AppUserId);

                ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Color).Include(c => c.Clothes)
                 .Include(c => c.ClothesColorSizes).ThenInclude(c => c.Size)
                .FirstOrDefaultAsync(c => c.ClothesId == cartItem.ClothesId
                && c.ColorId == cartItem.ColorId
                && c.ClothesColorSizes.Any(c => c.SizeId == cartItem.SizeId));

                
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);

                    string sizeName = clothesColor.ClothesColorSizes
                        .FirstOrDefault(c => c.SizeId == cartItem.SizeId).Size.Name;

                    TempData["Message"] = $"Product name:{clothesColor.Clothes.Name}/   Color:{clothesColor.Color.Name}/  Size:{sizeName}  has been removed";

                }
                await _context.SaveChangesAsync();

            }
            else
            {
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr)) return NotFound();

                CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
                CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                      .FirstOrDefault(c => c.Id == id && c.ColorId == colorId && c.SizeId == sizeId);


                ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Color).Include(c => c.Clothes)
                  .Include(c => c.ClothesColorSizes).ThenInclude(c => c.Size)
                 .FirstOrDefaultAsync(c => c.ClothesId == existedCookieItem.Id
                 && c.ColorId == existedCookieItem.ColorId
                 && c.ClothesColorSizes.Any(c => c.SizeId == existedCookieItem.SizeId));

                //sayi sifir olursa basketden silsin
                if (existedCookieItem.Quantity > 1)
                {
                    existedCookieItem.Quantity--;
                    cartCookie.TotalPrice -= clothesColor.Clothes.DiscountPrice;
                }
                else
                {
                    cartCookie.CartCookieItemVMs.Remove(existedCookieItem);
                    cartCookie.TotalPrice -= existedCookieItem.Quantity * clothesColor.Clothes.DiscountPrice;
                    string sizeName = clothesColor.ClothesColorSizes
                        .FirstOrDefault(c => c.SizeId == existedCookieItem.SizeId).Size.Name;

                    TempData["Message"] = $"Product name:{clothesColor.Clothes.Name}/   Color:{clothesColor.Color.Name}/  Size:{sizeName}  has been removed";

                }

                cartCookieStr = JsonConvert.SerializeObject(cartCookie);
                HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            }
            
            return RedirectToAction(nameof(Cart));
        }

      
    }
}
