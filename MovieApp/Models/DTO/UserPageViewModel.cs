using MovieApp.Models.Domain;

namespace MovieApp.Models.DTO
{
    public class UserPageViewModel
    {
        public string ProfileImage { get; set; }
        public float Balance { get; set; }
        public string UserType { get; set; }
        public string ExpirationDate {get;set;}
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        //public List<string> Movies { get; set; }
        //public List<string> MovieIds { get; set; }
        public Dictionary<int, string> Movies { get; set; }
        public IQueryable<Comment> Comments { get; set; }
        
    }
}
