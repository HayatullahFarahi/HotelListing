using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
    //TODO: AUTH
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        
        public DbSet<Hotel> Hotels { get; set; }
        
        //TODO:  seeding initial data to the database
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //TODO: AUTH
            base.OnModelCreating(builder);
            
            //TODO: seed country data to database
            builder.ApplyConfiguration(new CountryConfiguration());
            //TODO: seed hotel data to database
            builder.ApplyConfiguration(new HotelConfiguration());
            //TODO: SEEDING USER ROLES TO THE DATABASE
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}