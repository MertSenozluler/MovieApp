

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using System.Security.Claims;

namespace MovieApp.Controllers
{
    public class BuyController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DatabaseContext ctx;
        private readonly RoleManager<IdentityRole> roleManager;

        public BuyController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, DatabaseContext ctx, RoleManager<IdentityRole> roleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.ctx = ctx;
            this.roleManager = roleManager;
        }



        [HttpPost]
        public async Task<IActionResult> BuyBalance(float balance)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction("Login", "UserAuthentication");
            }
            else
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var loggedInUser = await userManager.FindByIdAsync(userId);
                var userName = loggedInUser?.UserName;

                var latestBalance = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault(); 
                if (User.IsInRole("admin"))
                {
                    TempData["msg"] = "Admin can't add money";
                    return RedirectToAction("TermsBuy", "Home");
                }
                    if (latestBalance == null)
                    {
                        var newUserBalanceMovie = new UserBalanceMovie
                        {
                            UserName = userName,
                            Balance = balance,
                            BuyMovie = null,
                            DateTime = DateTime.Now,
                            ExprationDate = null,
                            Role = "user"
                        };
                        ctx.UserBalanceMovie.Add(newUserBalanceMovie);

                    var adminBalance = new AdminBalance
                    {
                        DateTime = DateTime.Now,
                        UserName = userName,
                        Balance = balance

                    };
                        ctx.AdminBalance.Add(adminBalance);      
                        ctx.SaveChanges();
                        TempData["msg"] = "You bought successfully!";
                        return RedirectToAction("TermsBuy", "Home");
                 
                    }
                    else
                    {
                        var newUserBalanceMovie = new UserBalanceMovie
                        {
                            UserName = userName,
                            Balance = latestBalance.Balance + balance,
                            BuyMovie = latestBalance.BuyMovie,
                            DateTime = DateTime.Now,
                            ExprationDate = latestBalance.ExprationDate,
                            Role = latestBalance.Role
                        };
                        ctx.UserBalanceMovie.Add(newUserBalanceMovie);
                        var adminBalance = new AdminBalance
                        {
                        DateTime = DateTime.Now,
                        UserName = userName,
                        Balance = balance

                        };
                        ctx.AdminBalance.Add(adminBalance);
                        ctx.SaveChanges();
                        TempData["msg"] = "You bought successfully!";
                        return RedirectToAction("TermsBuy", "Home");
                    }           
            }


        }


        [HttpPost]
        public async Task<IActionResult> BuyPremium()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction("Login", "UserAuthentication");
            }

            else
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var loggedInUser = await userManager.FindByIdAsync(userId);
                var userName = loggedInUser?.UserName;

                var latestBalance = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault();

                if (latestBalance == null  || latestBalance.Balance < 30)
                {
                    TempData["msg"] = "Your balance is insufficient. please make balance first";
                    return RedirectToAction("TermsBuy","Home");
                }
                else
                {
                    //This users role changed. 
                    var roleExists = await roleManager.RoleExistsAsync("premiumUser");
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole("premiumUser"));
                    }

                    var userRoles = await userManager.GetRolesAsync(loggedInUser);
                    await userManager.RemoveFromRolesAsync(loggedInUser, userRoles);
                    await userManager.AddToRoleAsync(loggedInUser, "premiumUser");
                    var premiumExpiration = DateTime.Now.AddSeconds(120);
                    var finalBalance = latestBalance.Balance - 30;
                    var userBalanceMovie = new UserBalanceMovie
                    {
                        UserName = userName,
                        Balance = finalBalance,
                        BuyMovie = latestBalance.BuyMovie,
                        DateTime = DateTime.Now,
                        ExprationDate = premiumExpiration,
                        Role = "premiumUser",

                    };

                    ctx.UserBalanceMovie.Add(userBalanceMovie);
                    ctx.SaveChanges();

                    TempData["msg"] = "You are premium user! Enjoy all films";
                }

                
               
                return RedirectToAction("Home");

            }
        }

        [HttpPost]
        public async Task<IActionResult> BuySuper()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction("Login", "UserAuthentication");
            }

            else
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var loggedInUser = await userManager.FindByIdAsync(userId);
                var userName = loggedInUser?.UserName;

                var latestBalance = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault();



                if (latestBalance == null || latestBalance.Balance < 50)
                {
                    TempData["msg"] = "Your balance is insufficient. please make balance first";
                    return RedirectToAction("TermsBuy", "Home");
                }
                else
                {
                    // This users role changed. 
                    var roleExists = await roleManager.RoleExistsAsync("superUser");
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole("superUser"));
                    }

                    var userRoles = await userManager.GetRolesAsync(loggedInUser);
                    await userManager.RemoveFromRolesAsync(loggedInUser, userRoles);
                    await userManager.AddToRoleAsync(loggedInUser, "superUser");
                    var superExpiration = DateTime.Now.AddSeconds(120);
                    var finalBalance = latestBalance.Balance - 50;
                    var userBalanceMovie = new UserBalanceMovie
                    {
                        UserName = userName,
                        Balance = finalBalance,
                        BuyMovie = latestBalance.BuyMovie,
                        DateTime = DateTime.Now,
                        ExprationDate = superExpiration,
                        Role = "superUser",

                    };

                    ctx.UserBalanceMovie.Add(userBalanceMovie);
                    ctx.SaveChanges();

                    TempData["msg"] = "You are Super user! Enjoy all films";
                }



                return RedirectToAction("Home");

            }
        }

        [HttpPost]
        public async Task<IActionResult> BuyMovieDirect(int MovieId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await userManager.FindByIdAsync(userId);
            var userName = loggedInUser?.UserName;

            var latestBalance = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .FirstOrDefault();

            
            if (latestBalance != null)
            {
                if (latestBalance.BuyMovie != null)
                {
                    var boughtMovies = latestBalance.BuyMovie?.Split(',');
                    if (boughtMovies?.Contains(MovieId.ToString()) == true)
                    {
                        TempData["msg"] = "You already bought this movie.";
                        return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
                    }
                }
                var newUserBalanceMovie = new UserBalanceMovie
                {
                    UserName = userName,
                    Balance = latestBalance.Balance,
                    BuyMovie = latestBalance.BuyMovie == null ? "," + MovieId.ToString() : latestBalance.BuyMovie + "," + MovieId.ToString(),
                    DateTime = DateTime.Now,
                    ExprationDate = latestBalance.ExprationDate,
                    Role = latestBalance.Role
                };
                var adminBalance = new AdminBalance
                {
                    DateTime = DateTime.Now,
                    UserName = userName,
                    Balance = 1.5F
                };
                ctx.AdminBalance.Add(adminBalance);
                TempData["msg"] = "You bought successfully!";
                ctx.UserBalanceMovie.Add(newUserBalanceMovie);
                ctx.SaveChanges();
            }

            else if(latestBalance == null)
            {
                var newUserBalanceMovie = new UserBalanceMovie
                {
                    UserName = userName,
                    Balance = 0,
                    BuyMovie = "," + MovieId.ToString(),
                    DateTime = DateTime.Now,
                    ExprationDate = null,
                    Role = "user"
                };
                var adminBalance = new AdminBalance
                {
                    DateTime = DateTime.Now,
                    UserName = userName,
                    Balance = 1.5F
                };
                ctx.AdminBalance.Add(adminBalance);
                TempData["msg"] = "You bought successfully!";
                ctx.UserBalanceMovie.Add(newUserBalanceMovie);
                ctx.SaveChanges();
            }
            else
            {
                TempData["msg"] = "Something went wrong";
            }

            return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
        }

        [HttpPost]
        public async Task<IActionResult> BuyMovieBalance(int MovieId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
            }
            else
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var loggedInUser = await userManager.FindByIdAsync(userId);
                var userName = loggedInUser?.UserName;

                var latestBalance = ctx.UserBalanceMovie
                    .Where(x => x.UserName == userName)
                    .OrderByDescending(x => x.DateTime)
                    .FirstOrDefault();

                if (latestBalance != null && latestBalance.Balance > 1.5)
                {
                    if (latestBalance.BuyMovie != null)
                    {
                        var boughtMovies = latestBalance.BuyMovie?.Split(',');
                        if (boughtMovies?.Contains(MovieId.ToString()) == true)
                        {
                            TempData["msg"] = "You already bought this movie.";
                            return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
                        }
                    }
                    var newUserBalanceMovie = new UserBalanceMovie
                    {
                        UserName = userName,
                        Balance = (float)(latestBalance.Balance - 1.5),
                        BuyMovie = latestBalance.BuyMovie == null ? "," + MovieId.ToString() : latestBalance.BuyMovie + "," + MovieId.ToString(),
                        DateTime = DateTime.Now,
                        ExprationDate = latestBalance.ExprationDate,
                        Role = latestBalance.Role
                    };
                    ctx.UserBalanceMovie.Add(newUserBalanceMovie);
                    ctx.SaveChanges();
                    TempData["msg"] = "You bought successfully!";
                    return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
                }
                else
                {
                    TempData["msg"] = "Please buy balance first!";
                    return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId }); ;
                }
            }
        }

        public async Task<IActionResult> WatchMovie(string MovieName, int MovieId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["msg"] = "Please log in first.";
                return RedirectToAction("Login", "UserAuthentication");
            }
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await userManager.FindByIdAsync(userId);
            var userName = loggedInUser?.UserName;

            var latestBalance = ctx.UserBalanceMovie
            .Where(x => x.UserName == userName)
            .OrderByDescending(x => x.DateTime)
            .FirstOrDefault();

            if (latestBalance != null && latestBalance.Role != null)  ViewData["Role"] = latestBalance.Role;

            var movie = ctx.Movie.FirstOrDefault(x => x.MovieName == MovieName && x.Id == MovieId);
            if (movie == null)
            {
                TempData["msg"] = "The movie does not exist or Id and Name doesn't match. Please try again with using search button";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else if (User.IsInRole("admin") || latestBalance.Role == "superUser" || latestBalance.Role == "premiumUser" || (
                   !string.IsNullOrEmpty(latestBalance.BuyMovie) && latestBalance.BuyMovie.Split(',').Select(x => int.TryParse(x, out int MovieId) ? MovieId : 0).Contains(MovieId)))
            {
                var watchList = new WatchList
                {
                    UserName = userName,
                    DateTime = DateTime.Now,
                    MovieName = MovieName,
                };
                if (latestBalance != null)
                {
                    if (latestBalance.Role == "superUser") watchList.WatchType = "Super";

                    else if (latestBalance.Role == "premiumUser") watchList.WatchType = "Premium";

                    else if (latestBalance.Role == "user") watchList.WatchType = "User";
                }
                else if(User.IsInRole("admin")) watchList.WatchType = "Admin";

                else if (User.IsInRole("user")) watchList.WatchType = "User";


                ctx.WatchList.Add(watchList);
                ctx.SaveChanges();
                return View();
            }
            else
            {
                TempData["msg"] = "You should buy the movie or buy membership first!";
                return RedirectToAction(nameof(HomeController.MovieDetail), "Home", new { MovieId });
            }

          
        }

        

    }
}
