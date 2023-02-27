using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class UserAdminPageController : Controller
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserAdminService userAdminService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAdminPageController(DatabaseContext ctx, UserManager<ApplicationUser> userManager, IUserAdminService userAdminService, IHttpContextAccessor httpContextAccessor)
        {
            this.ctx = ctx;
            this.userManager = userManager;
            this.userAdminService = userAdminService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminPage()
        {
            var model = await userAdminService.AdminPage();
            return View(model);
        }

        [Authorize] 
        public async Task<IActionResult> UserPage(string term = "")
        {
            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail", "Home");
            }

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await userManager.FindByIdAsync(userId);
            var list = await userAdminService.UserPage(loggedInUser);
            return View(list);
        }

        
    }
}
