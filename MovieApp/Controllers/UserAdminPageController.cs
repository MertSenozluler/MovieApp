using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;

namespace MovieApp.Controllers
{
    public class UserAdminPageController : Controller
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserAdminService userAdminService;

        public UserAdminPageController(DatabaseContext ctx, UserManager<ApplicationUser> userManager, IUserAdminService userAdminService)
        {
            this.ctx = ctx;
            this.userManager = userManager;
            this.userAdminService = userAdminService;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminPage()
        {
            var model = await userAdminService.List();
            return View(model);
        }
    }
}
