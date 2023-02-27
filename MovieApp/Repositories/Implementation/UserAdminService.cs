using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using System.Globalization;
using System.Security.Claims;

namespace MovieApp.Repositories.Implementation
{
    public class UserAdminService : IUserAdminService
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAdminService(DatabaseContext ctx, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.ctx = ctx;
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AdminPageViewModel> AdminPage()
        {
            float Totalbalance = 0;
            var balanceList = ctx.AdminBalance.ToList();
            foreach (var balance in balanceList)
            {
                Totalbalance += balance.Balance;
            }
            var today = DateTime.Now;
            var oneDayAgo = today.AddDays(-1);
            float BalanceDaily = 0;
            var balancelist2 = ctx.AdminBalance.Where(x => x.DateTime >= oneDayAgo).Where(x => x.DateTime < today).ToList();
            foreach (var balance in balancelist2)
            {
                BalanceDaily += balance.Balance;
            }
            var oneWeekAgo = today.AddDays(-7);
            float BalanceWeek = 0;
            var balanceList3 = ctx.AdminBalance.Where(x => x.DateTime >= oneWeekAgo).Where(x => x.DateTime < today).ToList();
            foreach (var balance in balanceList3)
            {
                BalanceWeek += balance.Balance;
            }
            var TotalWatch = ctx.WatchList.Count();
            var TotalMovie = ctx.Movie.Count();         
            var CommentTotal = ctx.Comment.Count();
            var users = await ctx.Users.Where(u => u.IsActive).ToListAsync();
            int totalMembers = 0;
            int totalPremiumMembers = 0;
            int totalSuperMembers = 0;
            foreach (var user in users)
            {
                totalMembers++;

                var latestBalance = ctx.UserBalanceMovie
                .Where(x => x.UserName == user.UserName)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault();
                if (latestBalance != null && latestBalance.Role == "premiumUser")
                {
                    totalPremiumMembers++;
                }
                if (latestBalance != null && latestBalance.Role == "superUser")
                {
                    totalSuperMembers++;
                }
            }

            var list = new AdminPageViewModel
            {
                DailyBalance = BalanceDaily,
                WeeklyBalance = BalanceWeek,
                TotalBalance = Totalbalance,
                TotalMember = totalMembers,
                TotalPremiumMember= totalPremiumMembers,
                TotalSuperMember= totalSuperMembers,
                TotalMovie = TotalMovie,
                TotalWatch = TotalWatch,
                TotalComment = CommentTotal
            };
            return list;
        }

        public async Task<UserPageViewModel> UserPage(ApplicationUser loggedInUser)
        {
                         
                var loggedInUserProfilePicture = loggedInUser?.ProfilePicture;
                var userName = loggedInUser?.UserName;
                var userId = loggedInUser?.Id;
                var email = loggedInUser?.Email;
                var name = loggedInUser?.Name;

                var boughtMovies = ctx.UserBalanceMovie
                    .Where(x => x.UserName == userName)
                    .OrderByDescending(x => x.DateTime)
                    .Select(x => x.BuyMovie)
                    .FirstOrDefault();

                List<string> movieNames = null;
                List<int> movieIds = null;
                var boughtMovieIds = new List<int>();
                var movieDict = new Dictionary<int, string>();
                
                if (!string.IsNullOrEmpty(boughtMovies))
                {
                movieIds = boughtMovies.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(int.Parse) // convert string array to int list
                      .ToList();

                movieNames = ctx.Movie
                .Where(x => movieIds.Contains(x.Id))
                .Select(x => x.MovieName)
                .ToList();

                var movies = ctx.Movie
                .Where(x => movieIds.Contains(x.Id))
                .Select(x => new { x.Id, x.MovieName })
                .ToList();

                foreach (var movie in movies)
                {
                    movieDict[movie.Id] = movie.MovieName; 
                }

            }


                var role = ctx.UserBalanceMovie
                    .Where(x => x.UserName == userName)
                    .OrderByDescending(x => x.DateTime)
                    .Select(x => x.Role)
                    .FirstOrDefault();

                var expirationDate = ctx.UserBalanceMovie
                    .Where(x => x.UserName == userName)
                    .OrderByDescending(x => x.DateTime)
                    .Select(x => x.ExprationDate)
                    .FirstOrDefault();

                var balance = ctx.UserBalanceMovie
                    .Where(x => x.UserName == userName)
                    .OrderByDescending(x => x.DateTime)
                    .Select(x => x.Balance)
                    .FirstOrDefault();



            var userPage = new UserPageViewModel
                {
                    ProfileImage = loggedInUserProfilePicture,
                    Balance = balance,
                    UserType = role,
                    Movies = movieDict,
                    UserName = userName,
                    Name = name,
                    Email = email,
                    ExpirationDate = expirationDate.HasValue ? expirationDate.Value.ToString() : "No expiration date!",

            };
                return userPage;
          
            

            

        }
    }
}
