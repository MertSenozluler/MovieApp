namespace MovieApp.Models.Domain
{
    public class WatchList
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
        public string MovieName { get; set; }
        public string WatchType { get; set; }
    }
}
