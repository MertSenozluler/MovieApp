using Microsoft.AspNetCore.Identity;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using System.Security.Claims;
using System.Security.Policy;

namespace MovieApp.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public UserAuthenticationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;

        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }
            //match password
            if(!await userManager.CheckPasswordAsync(user,model.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid password";
                return status;
            }
            // false parameter :  the authentication session of the user will not persist after the browser is closed or the session times out.
            // true parameter : user will be locked out if the max allowed attempts for wrong password are reached. If it's set to false, user will not be locked out.
            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
        
            if(signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                foreach(var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                status.StatusCode = 1;
                status.Message = "Logged in successfully";         
                return status;
            }

            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User locked out";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on logging in";
                return status;
            }
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(RegistrationModel model)
        {
            var status = new Status();

            var emailExist = await userManager.FindByEmailAsync(model.Email);
            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (emailExist != null)
            {
                status.StatusCode = 0;
                status.Message = "Email already exist";
                return status;
            }

            
            if(userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "Username already exist";
                return status;
            }

            

            ApplicationUser user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.UserName,
                Email = model.Email,
                UserName = model.UserName,
                EmailConfirmed = true,
                IsActive=true,
            };

      

            var result = await userManager.CreateAsync(user,model.Password);
            if(!result.Succeeded) 
            {
                status.StatusCode = 0;
                status.Message = "User creation failed";
                return status;
            }

            // role management
            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));
            if (await roleManager.RoleExistsAsync(model.Role))
                await userManager.AddToRoleAsync(user, model.Role);


            status.StatusCode = 1;
            status.Message = "User has registered successfully";
            return status;
        }

        
    }
}
