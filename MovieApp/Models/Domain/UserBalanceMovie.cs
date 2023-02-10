namespace MovieApp.Models.Domain
{
    public class UserBalanceMovie
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public float Balance { get; set; }
        public string? BuyMovie { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? ExprationDate { get; set; }
        public string? Role { get; set; }
    }
}
