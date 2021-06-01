using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
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