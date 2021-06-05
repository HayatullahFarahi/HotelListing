using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models
{
    // user should not directly interact with the database models in the data folder
    // instead we create ViewModels for users to interact with and map them to the db models
    public class CreateCountryDTO
    {
        
        [Required]
        [StringLength(maximumLength: 50, ErrorMessage = "Country name is too long")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(maximumLength: 3, ErrorMessage = "Short country name is too long")]
        public string ShortName { get; set; }
        
    }
    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }
        public  virtual IList<HotelDTO> Hotels { get; set; }
    }

    public class UpdateCountryDTO : CreateCountryDTO
    {
        
    }
}