namespace MovieApp.Models.DTO
{
    public class ListUsersViewModel
    {
        public string UserId { get; set; }
        public string ProfilePicture { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public float Balance { get; set; }
        public string Movies { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Term { get; set; }

    }
}
