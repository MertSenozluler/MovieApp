using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieApp.Models.Domain
{
    public class DatabaseContext: IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movie { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<MovieCategory> MovieCategory { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<MovieSubCategory> MovieSubCategory { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<MovieCountry> MovieCountry { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserBalanceMovie> UserBalanceMovie { get; set; }
        public DbSet<AdminBalance> AdminBalance { get; set; }
        public DbSet<WatchList> WatchList { get; set; }

    }
}
