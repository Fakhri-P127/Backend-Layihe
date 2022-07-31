using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels;
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
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(ApplicationDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ProductVM model = new ProductVM
            {
                Clothes = await _context.Clothes.Include(c=>c.ClothesImages)
                .Include(c=>c.ClothesColors).ThenInclude(c=>c.ClothesColorSizes)
                .FirstOrDefaultAsync(c => c.Id == id),
                Clotheses = new List<Clothes>(),
                ClothesColorSizes = await _context.ClothesColorSizes.Include(c => c.Size).Include(c => c.ClothesColor)
                .ThenInclude(c => c.Clothes).Where(s => s.ClothesColor.ClothesId == id).ToListAsync()
            };

            if (model.Clothes is null || model.ClothesColorSizes is null) return NotFound();

            List<Clothes> clothes = new List<Clothes>();
            Category category = await _context.Categories.Include(c => c.Clothes)
                .FirstOrDefaultAsync(c => c.Clothes.Any(d => c.Id == d.CategoryId));

            foreach (Clothes product in category.Clothes)
            {
                clothes = await _context.Clothes.Include(x => x.Category).ThenInclude(c => c.Clothes)
                    .Include(x => x.ClothesImages)
                    .Where(p => product.CategoryId == p.CategoryId && p.Id !=id).ToListAsync();
                    
                model.Clotheses.AddRange(clothes);
            }
            model.Clotheses = model.Clotheses.Distinct().ToList();

            //olan datalarimla muqasiye uchun
            ViewBag.Sizes = await _context.Sizes.Include(s => s.ClothesColorSizes).ThenInclude(s => s.ClothesColor)
                .ThenInclude(s => s.Clothes).ToListAsync();
            ViewBag.Colors = await _context.Colors.Include(c => c.ClothesColors)
                 .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToListAsync();

            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Detail")]
        public async Task<IActionResult> AddCart(int? id, int ColorId, int SizeId, byte Quantity)
        {
            if (id is null || id == 0) return NotFound();
            ProductVM model = new ProductVM
            {
                Clothes = await _context.Clothes.Include(c => c.ClothesImages)
                .FirstOrDefaultAsync(c => c.Id == id)
            };
            if (model.Clothes is null) return NotFound();


            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user is null) return NotFound();
                user.CartItems = await _context.CartItems
                  .Include(b => b.AppUser).Include(b => b.Clothes)
                  .Where(b => b.AppUserId == user.Id).ToListAsync();

                CartItem cartItem = await _context.CartItems.Include(c => c.AppUser)
                    .Include(c => c.Clothes).ThenInclude(c => c.ClothesImages)
                    .FirstOrDefaultAsync(c => c.ClothesId == id &&
                    user.Id == c.AppUserId && c.ColorId == ColorId && c.SizeId == SizeId);
                //Clothes clothes = await _context.Clothes.Include(c => c.ClothesImages)
                //    .FirstOrDefaultAsync(c => c.Id == id );

                if (cartItem is null)
                {
                    cartItem = new CartItem
                    {
                        AppUser = user,
                        Clothes = model.Clothes,
                        Quantity = Quantity,
                        Price = model.Clothes.DiscountPrice,
                        ColorId = ColorId,
                        SizeId = SizeId,
                    };
                    _context.CartItems.Add(cartItem);

                }
                else
                {
                    cartItem.Quantity++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                CartCookieVM cartCookie;
                string cartCookieStr = HttpContext.Request.Cookies["Cart"];

                // if cookie is empty
                if (string.IsNullOrEmpty(cartCookieStr))
                {
                    cartCookie = new CartCookieVM();
                    cartCookie.CartCookieItemVMs = new List<CartCookieItemVM>();

                    CartCookieItemVM cartCookieItem = new CartCookieItemVM
                    {
                        Id = model.Clothes.Id,
                        Quantity = Quantity,
                        ColorId = ColorId,
                        SizeId = SizeId
                    };
                    cartCookie.CartCookieItemVMs.Add(cartCookieItem);

                    //if(model.Clothes.DiscountPrice is null)
                    //{
                    cartCookie.TotalPrice = model.Clothes.DiscountPrice * Quantity;
                    //}
                    //else
                    //{
                    //    cartCookie.TotalPrice = (decimal)(model.Clothes.Price/model.Clothes.DiscountPrice * Quantity);

                    //}
                    //cartCookie.TotalPrice = model.Clothes.Price * cartCookieItem.Quantity;
                }
                // if cookie has items
                else
                {
                    cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(cartCookieStr);
                    CartCookieItemVM existedCookieItem = cartCookie.CartCookieItemVMs
                        .FirstOrDefault(c => c.Id == model.Clothes.Id && c.ColorId == ColorId && c.SizeId == SizeId);

                    // if item doesn't exist in cart
                    if (existedCookieItem is null)
                    {
                        CartCookieItemVM cartCookieItem = new CartCookieItemVM
                        {
                            Id = model.Clothes.Id,
                            Quantity = Quantity,
                            ColorId = ColorId,
                            SizeId = SizeId
                        };
                        cartCookie.CartCookieItemVMs.Add(cartCookieItem);
                        cartCookie.TotalPrice += model.Clothes.DiscountPrice * Quantity;
                    }
                    // if item exists in cart
                    else
                    {
                        existedCookieItem.Quantity += Quantity;
                        cartCookie.TotalPrice += model.Clothes.DiscountPrice * Quantity;
                    }
                }

                cartCookieStr = JsonConvert.SerializeObject(cartCookie);
                HttpContext.Response.Cookies.Append("Cart", cartCookieStr);
            }

            return RedirectToAction("Cart", "Order");

        }


        public IActionResult ShowBasket()
        {
            if (HttpContext.Request.Cookies["Cart"] is null) return NotFound();
            CartCookieVM cart = JsonConvert.DeserializeObject<CartCookieVM>(HttpContext.Request.Cookies["Cart"]);
            return Json(cart);
        }

        // Fetch method for size sorting in product detail
        public async Task<IActionResult> GetDatas(int? clothesId, int? colorId)
        {
            Clothes existed = await _context.Clothes.FirstOrDefaultAsync(c => c.Id == clothesId);
            Color color = await _context.Colors.Include(c => c.ClothesColors)
                .FirstOrDefaultAsync(c => c.Id == colorId);
            if (color is null) return NotFound();
            ClothesColor clothesColor = await _context.ClothesColors
                .Include(c=>c.ClothesColorSizes).ThenInclude(c=>c.Size)
                .FirstOrDefaultAsync(c => c.ColorId == colorId && existed.Id == c.ClothesId);
            List<ClothesColorSize> clothesColorSize = await _context.ClothesColorSizes.Include(c => c.Size)
                .Where(c => c.ClothesColorId == clothesColor.Id).ToListAsync();

            return PartialView("_SizesFetchPartialView", clothesColorSize);
           
        }

     
    }
}
