using Microsoft.AspNetCore.Identity;

namespace HotelListing.Data
{
    //TODO: AUTH
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}