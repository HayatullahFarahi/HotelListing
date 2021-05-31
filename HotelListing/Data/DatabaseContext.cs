using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        
        public DbSet<Hotel> Hotels { get; set; }
        
        //TODO:  seeding initial data to the database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "Afghanistan",
                    ShortName = "AFG"
                }, 
                new Country
                {
                    Id = 2,
                    Name = "India",
                    ShortName = "IN"
                }, 
                new Country
                {
                    Id = 3,
                    Name = "Turkey",
                    ShortName = "TR"
                }
                );
            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Continental",
                    Address = "Kabul",
                    CountryId = 1,
                    Rating = 4.5
                }, 
                new Hotel
                {
                    Id = 2,
                    Name = "Natasha",
                    Address = "Goa, India",
                    CountryId = 2,
                    Rating = 4
                }, 
                new Hotel
                {
                    Id = 3,
                    Name = "Istanbul Hotel",
                    Address = "Istanbul",
                    CountryId = 3,
                    Rating = 4.3
                }
                );
        }
    }
}