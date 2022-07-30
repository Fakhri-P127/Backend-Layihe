using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.Service;
using Backend_MVC_Layihe.ViewModels.Account;
using Backend_MVC_Layihe.ViewModels.Cart;
using Microsoft.AspNetCore.Http;
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
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleInManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
               RoleManager<IdentityRole> roleInManager, ApplicationDbContext context, IHttpContextAccessor http)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleInManager = roleInManager;
            _context = context;
            _http = http;
    
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            if(!register.Terms)
            {
                ModelState.AddModelError(string.Empty, "Please check our terms");
                return View();
            }
            AppUser user = new AppUser
            {
                Firstname = register.Firstname,
                Lastname = register.Lastname,
                UserName = register.Username,
                Email = register.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, "Member");
            

            return RedirectToAction("Home", "Index");
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(login.Username);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Username or password incorrect.");
                return View();
            } 
            user.BasketItems = await _context.BasketItems.Include(b => b.AppUser).Include(b => b.Clothes)
                .Where(b => b.AppUserId == user.Id).ToListAsync();
            
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager
                .PasswordSignInAsync(user, login.Password, login.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "You made too many attempts. Wait for a while");
                    return View();
                }
                ModelState.AddModelError(string.Empty, "Username or password incorrect.");
                return View();
            }

            string cartCookieStr = HttpContext.Request.Cookies["Cart"];
            CartCookieVM cartCookie;
            if (!string.IsNullOrEmpty(cartCookieStr))
            {
                cartCookie = JsonConvert.DeserializeObject<CartCookieVM>(cartCookieStr);
                foreach (CartCookieItemVM cookieItem in cartCookie.CartCookieItemVMs)
                {
                    // Eger eyni mehsuldan userBasket de varsa onda quantitysini artir, birde mehsulu yeniden yaratma.
                    CartItem existedBasketItem = user.BasketItems.FirstOrDefault(b => b.ClothesId == cookieItem.Id 
                    && b.SizeId== cookieItem.SizeId && b.ColorId == cookieItem.ColorId);
                    if(existedBasketItem != null)
                    {
                        existedBasketItem.Quantity += cookieItem.Quantity;
                        continue;
                    }
                    
                    //Clothes existedClothes = await _context.Clothes.FirstOrDefaultAsync(p => p.Id == cookieItem.Id);
                    ClothesColor clothesColor = await _context.ClothesColors.Include(c => c.Clothes)
               .Include(c => c.ClothesColorSizes)
              .FirstOrDefaultAsync(c => c.ClothesId == cookieItem.Id
              && c.ColorId == cookieItem.ColorId
              && c.ClothesColorSizes.Any(c => c.SizeId == cookieItem.SizeId));

                    if (clothesColor is null) return NotFound();

                    CartItem basketItem = new CartItem
                    {
                        Quantity = cookieItem.Quantity,
                        Clothes = clothesColor.Clothes,
                        AppUser = user,
                        Price = clothesColor.Clothes.Price,
                        ColorId=cookieItem.ColorId,
                        SizeId=cookieItem.SizeId
                    };
                    _context.BasketItems.Add(basketItem);

                    // layout service den instance almaqimin sebebi odur ki, normalda login olanda cookie deki datalardan basketItem yaradiriq(Database) amma bunlar basket e dushmur. Bunun uchun getBasket() metodunu cagiririq. 
                    
                    LayoutService layoutService = new LayoutService(_context, _http, _userManager);
                    await layoutService.GetCartCount();
                }
                HttpContext.Response.Cookies.Delete("Cart");
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public JsonResult ShowAuthentication()
        {
            return Json(User.Identity.IsAuthenticated);
        }

        public async Task CreateRoles()
        {
            await _roleInManager.CreateAsync(new IdentityRole("Member"));
            await _roleInManager.CreateAsync(new IdentityRole("Moderator"));
            await _roleInManager.CreateAsync(new IdentityRole("Admin"));
        }

    }
}
