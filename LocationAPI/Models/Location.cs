using static System.Reflection.Metadata.BlobBuilder;

namespace LocationAPI.Models
{
    public class Location
    {
        public int CityID { get; set; }  // Unique identifier for the location (city)
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Country { get; set; } // The country associated with the city
    }
}
