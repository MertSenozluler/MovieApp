using MovieApp.Models.Domain;

namespace MovieApp.Models.DTO
{
    public class MovieCommentViewModel
    {
        // for recommended movie section
        public IQueryable<Movie> MovieList { get; set; }
        // for one movie according to movieId
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string MovieDescription { get; set; }
        public string Actors { get; set; }
        public string PublishYear { get; set; }
        public string? MovieImage { get; set; }
        public string Categories { get; set; }       

        // Comment Section
        public IQueryable<Comment> CommentList { get; set; }

        // UserId is the Id of the logged in user
        public string UserId { get; set; }
        public string LoggedInUserProfilePicture { get; set; }

        // according to this, users will see or not see watch button
        public string BuyMovie { get; set; }
        public string Role { get; set; }
    }
}

