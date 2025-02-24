//using LocationAPI.Models;


using LocationAPI.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace LocationAPI.Services
{
    public interface ILocationService
    {
        IEnumerable<Location> GetLocations(string country);
        Location AddLocation(Location newLocation);
        bool UpdateLocation(int cityID, Location updatedLocation, int countryID);
        bool DeleteLocation(Location location);
        int GetCountryID(string country);
    }

}
