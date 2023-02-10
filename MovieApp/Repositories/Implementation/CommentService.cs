using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models.Domain;
using MovieApp.Models.DTO;
using MovieApp.Repositories.Abstract;
using System.Security.Claims;

namespace MovieApp.Repositories.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly DatabaseContext ctx;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(DatabaseContext ctx, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.ctx = ctx;
            _userManager = userManager;
            // this is for getting login userid
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Add(Comment model, string userId, int MovieId)
        {           
            try
            {
                var user = ctx.Users.Find(userId);
                model.UserId = userId;
                model.CommentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                model.MovieId = MovieId;
                ctx.Comment.Add(model);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int CommentId, string CommentUserId)
        {
            try
            {
                var comment = ctx.Comment.FirstOrDefault(x => x.Id == CommentId);
                if (comment == null || comment.UserId == null) return false;

                var CommentUser = ctx.Users.Find(CommentUserId);
                if (CommentUser == null) return false;

                // login user id received
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // also admin can delete every comment
                if (CommentUserId == userId || _httpContextAccessor.HttpContext.User.IsInRole("admin"))
                {
                    ctx.Comment.Remove(comment);
                    ctx.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<MovieCommentViewModel> List(int MovieId, string term = "")
        {
          
            // This is used to get movie detail
            var movie = ctx.Movie.FirstOrDefault(m => m.Id == MovieId);

            if (movie == null)
            {
                return null;
            }
          
            // Category Select for this movie
            var movieCategories = ctx.MovieCategory
                .Where(mc => mc.MovieId == movie.Id)
                .Select(mc => mc.CategoryId).ToList();
            var categories = ctx.Category.Where(c => movieCategories
                .Contains(c.Id))
                .Select(c => c.CategoryName).ToList();
                
            //this is used to get the profile picture of the logged in user
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = await Task.Run(() => _userManager.FindByIdAsync(userId).Result);
            var loggedInUserProfilePicture = loggedInUser?.ProfilePicture;
            var userName = loggedInUser?.UserName;
            var boughtMovies = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .Select(x => x.BuyMovie)
                .FirstOrDefault();
            var role = ctx.UserBalanceMovie
                .Where(x => x.UserName == userName)
                .OrderByDescending(x => x.DateTime)
                .Select(x => x.Role)
                .FirstOrDefault();
            //this is used to make "you may also like" section
            var movieList = ctx.Movie.AsQueryable();

            var comments = ctx.Comment.Where(c => c.MovieId == movie.Id).AsQueryable();


            if (comments.Count() == 0)
            {
                var noComment = new MovieCommentViewModel
                {

                    MovieId = movie.Id,
                    MovieName = movie.MovieName,
                    MovieDescription = movie.MovieDescription,
                    PublishYear = movie.PublishYear,
                    Actors = movie.Actors,
                    MovieImage = movie.MovieImage,
                    Categories = string.Join(", ", categories),
                    MovieList = movieList,
                    LoggedInUserProfilePicture = loggedInUserProfilePicture,
                    BuyMovie = boughtMovies,
                    Role = role
                };
                return noComment;
            }

            foreach (var comment in comments)
            {
                var user = await Task.Run(() => _userManager.FindByIdAsync(comment.UserId));
                comment.UserName = user?.UserName;
                comment.UserProfilePicture = user?.ProfilePicture;
            }

            var result = new MovieCommentViewModel
            {
                MovieList = movieList,
                MovieId = movie.Id,
                MovieName = movie.MovieName,
                MovieDescription = movie.MovieDescription,
                Actors = movie.Actors,
                PublishYear = movie.PublishYear,
                MovieImage = movie.MovieImage,
                Categories = string.Join(", ", categories),
                CommentList = comments,
                LoggedInUserProfilePicture = loggedInUserProfilePicture,
                UserId = userId,
                BuyMovie = boughtMovies,
                Role = role

            };

            return result;
        }

        
    }
}
