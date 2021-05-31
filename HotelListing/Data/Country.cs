using System.Collections;
using System.Collections.Generic;

namespace HotelListing.Data
{
    public class Country
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string ShortName { get; set; }
        
        // it will give list of related hotels to the country
        // it will not reflect in the tables; virtual
        public virtual IList<Hotel> Hotels { get; set; }
    }
}