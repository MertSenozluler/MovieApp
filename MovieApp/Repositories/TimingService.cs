
using Microsoft.AspNetCore.Identity;
using MovieApp.Models.Domain;
using System.Threading;

namespace MovieApp.Repositories
{
    public class TimingService : BackgroundService
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TimingService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }



        private async Task EndDate(DatabaseContext ctx, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var users = ctx.UserBalanceMovie.Select(x => x.UserName).Distinct();
            foreach (var userName in users)
            {
                var user = await userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    var finalBalance = ctx.UserBalanceMovie
                        .Where(x => x.UserName == userName)
                        .OrderByDescending(x => x.DateTime)
                        .FirstOrDefault();

                    if (finalBalance.ExprationDate.HasValue && finalBalance.ExprationDate.Value < DateTime.Now)
                    {
                        // If the balance is sufficient, the package will be renewed monthly.

                        if (finalBalance.Balance >= 50 && finalBalance.Role == "superUser")
                        {
                            var userRoles = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, userRoles);
                            await userManager.AddToRoleAsync(user, "superUser");
                            var userBalanceMovie = new UserBalanceMovie
                            {
                                UserName = userName,
                                Balance = finalBalance.Balance - 50,
                                BuyMovie = finalBalance.BuyMovie,
                                DateTime = DateTime.Now,
                                ExprationDate = DateTime.Now.AddSeconds(20),
                                Role = "superUser"
                            };
                            ctx.UserBalanceMovie.Add(userBalanceMovie);
                            ctx.SaveChanges();
                        }

                        else if (finalBalance.Balance >= 30 && finalBalance.Role == "premiumUser")
                        {
                            var userRoles = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, userRoles);
                            await userManager.AddToRoleAsync(user, "premiumUser");
                            var userBalanceMovie = new UserBalanceMovie
                            {
                                UserName = userName,
                                Balance = finalBalance.Balance - 30,
                                BuyMovie = finalBalance.BuyMovie,
                                DateTime = DateTime.Now,
                                ExprationDate = DateTime.Now.AddSeconds(120),
                                Role = "premiumUser"
                            };
                            ctx.UserBalanceMovie.Add(userBalanceMovie);
                            ctx.SaveChanges();
                        }

                        else
                        {
                            var userRoles = await userManager.GetRolesAsync(user);
                            await userManager.RemoveFromRolesAsync(user, userRoles);
                            await userManager.AddToRoleAsync(user, "user");
                            var userBalanceMovie = new UserBalanceMovie
                            {
                                UserName = userName,
                                Balance = finalBalance.Balance,
                                BuyMovie = finalBalance.BuyMovie,
                                DateTime = DateTime.Now,
                                ExprationDate = null,
                                Role = "user"
                            };
                            ctx.UserBalanceMovie.Add(userBalanceMovie);
                            ctx.SaveChanges();
                        }

                    }
                }
            }
        }



        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await EndDate(ctx, roleManager, userManager);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }


}
