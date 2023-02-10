using MovieApp.Models.Domain;
using MovieApp.Models.DTO;

namespace MovieApp.Repositories.Abstract
{
    public interface ICommentService
    {
        bool Add(Comment model, string userId, int MovieId);
        Task<bool> Delete(int CommentId, string CommentUserId);
        Task<MovieCommentViewModel> List(int MovieId, string term = "");
    }
}
