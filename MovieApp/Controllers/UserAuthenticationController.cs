using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using MovieApp.Repositories.Implementation;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService _service;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext _context;
        private readonly IFileService fileService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public UserAuthenticationController(IUserAuthenticationService service, UserManager<ApplicationUser> userManager, DatabaseContext context, IFileService fileService, RoleManager<IdentityRole> roleManager, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            this.userManager = userManager;
            _context = context;
            this.fileService = fileService;
            this.roleManager = roleManager;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }


        //this method used for only creating admin
        //public async Task<IActionResult> reg()
        //{
        //    var model = new RegistrationModel
        //    {
        //        UserName = "admin",
        //        Name = "admin",
        //        Email = "admin@admin.com",
        //        Password = "Admin@12345#"
        //        IsActive = true,
        //        ProfilImagae = "admin.jpg"
        //    };
        //    model.Role = "admin";
        //    var result = await _service.RegistrationAsync(model);
        //    return Ok(result);
        //}


        public IActionResult Login(string term = "")
        {
            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
                return View(model);          
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && user.IsActive == false)
            {
                TempData["msg"] = "Your account has been deactivated. Please contact the administrator.";
                return RedirectToAction(nameof(Login));
            }
            var result = await _service.LoginAsync(model);
            if (result.StatusCode== 1)
            {
                TempData["msg"] = "Welcome!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Registration(string term="")
        {
            if (!string.IsNullOrEmpty(term))
            {
                TempData["term"] = term;
                return RedirectToAction("SearchResultInDetail", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
                return View(model);

            model.Role = "user";
            if (model.ImageFile != null)
            {
                var fileResult = this.fileService.
                    SaveImage(model.ImageFile);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                var imageName = fileResult.Item2;
                model.ProfileImage = imageName;
            }
            else
            {
                model.ProfileImage = "default.jpg";
            }
            var result = await _service.RegistrationAsync(model);
            status = result;
            // save this image to ProfileImage in ApplicationUser
            var user = await userManager.FindByEmailAsync(model.Email);
            TempData["msg"] = result.Message;
            if (user == null) return View(model);
            user.ProfilePicture = model.ProfileImage;
            await userManager.UpdateAsync(user);
            if (status.StatusCode == 0) return View(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ModelState.Clear();
            return RedirectToAction("ConfirmEmail", new { email = model.Email });


        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _service.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ListUsers(string term = "", int currentPage = 1)
        {
            var data = await _userService.List(term, true, currentPage);
            
            return View(data);
        }

        // this method used for change user's status active or not active by Admin
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult IsActive(string UserName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName);
            if (user == null)
            {
                TempData["Message"] = "User can't found";
                return RedirectToAction("ListUsers", "UserAuthentication");
            }
            if (User.IsInRole("admin") && UserName == "admin")
            {
                TempData["Message"] = "You can't change the activity status of an admin user";
                return RedirectToAction("ListUsers", "UserAuthentication");
            }
            user.IsActive = !user.IsActive;
            _context.SaveChanges();
            TempData["Message"] = "users activity status has been successfully changed";
            return RedirectToAction("ListUsers", "UserAuthentication");
        }

       
        public async Task<IActionResult> GetCurrentUserRole()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await userManager.FindByIdAsync(userId);
            var currentUserRole = "";
            if (loggedInUser != null)
            {
                currentUserRole = (await userManager.GetRolesAsync(loggedInUser)).FirstOrDefault();
            }
           
            return Json(currentUserRole);           
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ViewBag.IsSuccess = false;
                    return View(model);
                }

            }
            else
            {
                TempData["msg"] = "Something went wrong. Please try again later";
                return View(model);
            }          

        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token, string email)
        {
            EmailConfirmModel model = new EmailConfirmModel
            {
                Email = email
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                // delete space in token
                token = token.Replace(' ', '+');
                var result = await _service.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }

            return View(model);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified = true;
                    return View(model);
                }

                await _service.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _service.GenerateForgotPasswordTokenAsync(user);
                }

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel resetPasswordModel = new ResetPasswordModel
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _service.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ChangeProfilePicture(IFormFile file)
        {
            
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await userManager.FindByIdAsync(userId);

            if (file != null)
            {
                // Save the image file
                var fileResult = fileService.SaveImage(file);
                if (fileResult.Item1 == 0)
                {
                    TempData["msg"] = "The file could not be saved";
                    return RedirectToAction("UserPage", "UserAdminPage");
                }
                var imageName = fileResult.Item2;
            
                // Update the user's profile picture
                loggedInUser.ProfilePicture = imageName;
                var result = await userManager.UpdateAsync(loggedInUser);
                if (result.Succeeded)
                    TempData["msg"] = "Profile picture updated successfully";
                else
                    TempData["msg"] = "An error occurred while updating the profile picture";

                return RedirectToAction("UserPage", "UserAdminPage");
            }
            else
            {
                TempData["msg"] = "You must upload the file first";
                return RedirectToAction("UserPage", "UserAdminPage");
            }

            
        }






    }
}
