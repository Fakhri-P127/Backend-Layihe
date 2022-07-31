using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Service
{
    public class LayoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(ApplicationDbContext context,IHttpContextAccessor http,UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;
        }
        public List<Setting> GetSettings()
        {
            List<Setting> settings = _context.Settings.ToList();
            return settings;
        }
        public List<Category> GetCategories()
        {
            List<Category> categories = _context.Categories.ToList();
            return categories;
        }

        public async Task<int> GetCartCount()
        {
            if (_http.HttpContext.User.Identity.IsAuthenticated && _http.HttpContext.User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
                user.CartItems = await _context.CartItems.Include(c => c.AppUser).Include(c => c.Clothes)
                    .Where(c => c.AppUserId == user.Id).ToListAsync();

                if(user.CartItems is null)
                {
                    user.CartItems = new List<CartItem>();
                }
                return user.CartItems.Count;
            }
            else
            {
                string cartCookieStr = _http.HttpContext.Request.Cookies["Cart"];
                if (string.IsNullOrEmpty(cartCookieStr))
                {
                    return 0;
                }
                else
                {
                    CartCookieVM cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(cartCookieStr);
                    return cartCookie.CartCookieItemVMs.Count;
                }
            }
           
        }
        public async Task<int> GetWishlistCount()
        {
            if (_http.HttpContext.User.Identity.IsAuthenticated && _http.HttpContext.User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(_http.HttpContext.User.Identity.Name);
                user.WishlistItems = await _context.WishlistItems.Include(c => c.AppUser).Include(c => c.Clothes)
                    .Where(c => c.AppUserId == user.Id).ToListAsync();

                if (user.WishlistItems is null)
                {
                    user.WishlistItems = new List<WishlistItem>();
                }
                return user.WishlistItems.Count;
            }
            else
            {
                return 0;
            }

        }
    }
}
