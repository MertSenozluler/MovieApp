using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using System.Globalization;

namespace MovieApp.Repositories.Implementation
{
    public class UserAdminService : IUserAdminService
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> userManager;

        public UserAdminService(DatabaseContext ctx, UserManager<ApplicationUser> userManager)
        {
            this.ctx = ctx;
            this.userManager = userManager;
        }

        public async Task<AdminPageViewModel> List()
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
    }
}
