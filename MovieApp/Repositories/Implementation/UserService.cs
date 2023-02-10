using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using System.Collections.Generic;

namespace MovieApp.Repositories.Implementation
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserService(DatabaseContext ctx, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.ctx = ctx;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        public async Task<List<ListUsersViewModel>> List(string term = "", bool paging = false, int currentPage = 0)
        {
            var userBalances = ctx.UserBalanceMovie.ToList();

            // I used Task.Run() for not getting life circle error 
            var users = await Task.Run(() => userManager.Users.Select(user => new ListUsersViewModel
            {
                ProfilePicture = user.ProfilePicture,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userManager.GetRolesAsync(user).Result,
                IsActive = user.IsActive,
            }).ToList());


            foreach (var user in users)
            {
                var balanceMovie = userBalances.Where(ub => ub.UserName == user.UserName)
                .OrderByDescending(ub => ub.DateTime)
                .FirstOrDefault();

                if (balanceMovie != null)
                {
                    user.Balance = balanceMovie.Balance;
                    user.Movies = balanceMovie.BuyMovie;
                }
                else
                {
                    user.Balance = 0;
                    user.Movies = "No Movie";
                }
            }

            //search button
            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                users = users.Where(a => a.UserName.ToLower().Contains(term) ||  a.Name.ToLower().Contains(term) || a.Email.ToLower().Contains(term)).ToList();
   

            }
    
            // pagination
            if (paging)
            {
                int pageSize = 5;
                int count = users.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                users = users.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                foreach (var user in users)
                {
                    user.PageSize = pageSize;
                    user.CurrentPage = currentPage;
                    user.TotalPages = TotalPages;
                }
            }

            return users;
        }

        
    }
    }

