using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public int? MovieId { get; set; }
        public string? CommentDate { get; set; }
        public string Body { get; set; }
        public string? UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string UserProfilePicture { get; set; }
     
    }
}
