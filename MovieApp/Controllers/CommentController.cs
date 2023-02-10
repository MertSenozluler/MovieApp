using Microsoft.AspNetCore.Mvc;
using MovieApp.Models.Domain;
using MovieApp.Repositories.Abstract;
using System.Xml.Linq;

namespace MovieApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public IActionResult Add(Comment model, string userId, int MovieId)
        {
        
            var result = _commentService.Add(model, userId, MovieId);
            if (result)
            {
                TempData["msg"] = "Added successfully";
                return RedirectToAction("MovieDetail", "Home", model);
            }
            else
            {
                TempData["msg"] = "Comment can't be null. Please try again";
                return RedirectToAction("MovieDetail", "Home", model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int CommentId, string CommentUserId, Comment model)
        {
            _commentService.Delete(CommentId, CommentUserId);
            TempData["msg"] = "Successfully deleted";
            return RedirectToAction("MovieDetail", "Home", model);
        }
    }
}
