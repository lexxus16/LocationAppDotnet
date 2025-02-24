using LocationAPI.Models;
using LocationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
//using LocationAPI.Models;
//using LocationAPI.Services;

namespace LocationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/Location/{country}
        [HttpGet("{country}")]
        public IActionResult GetLocations(string country)
        {
            var locations = _locationService.GetLocations(country); // Assuming GetLocations fetches locations based on the country name.
            return Ok(locations);
        }

        // POST: api/Location
        [HttpPost]
        public IActionResult AddLocation([FromBody] Location newLocation)
        {
            if (newLocation == null)
            {
                return BadRequest("Invalid location data.");
            }

            var createdLocation = _locationService.AddLocation(newLocation);
            return CreatedAtAction(nameof(GetLocations), new { country = createdLocation.Country }, createdLocation);
        }

        // PUT: api/Location/{cityID}
        [HttpPut("{cityID}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateLocation(int cityID, [FromBody] Location updatedLocation)
        {
            if (updatedLocation == null || updatedLocation.CityID != cityID)
            {
                return BadRequest("Location data is invalid.");
            }

            // Get CountryID from the country name
            int countryID = _locationService.GetCountryID(updatedLocation.Country);
            if (countryID == 0)
            {
                return BadRequest("Invalid country name.");
            }

            // Call service to update city location with the new CountryID
            bool success = _locationService.UpdateLocation(cityID, updatedLocation, countryID);
            if (!success)
            {
                return NotFound("Location not found.");
            }

            return NoContent();  // Successful update
        }

        // DELETE: api/Location
        [HttpDelete]
        public IActionResult DeleteLocation([FromBody] Location location) // Accept Location object from body
        {
            if (location == null || string.IsNullOrEmpty(location.City) || string.IsNullOrEmpty(location.PostalCode) || string.IsNullOrEmpty(location.Province))
            {
                return BadRequest("Invalid location data."); // Validate the Location object
            }

            bool success = _locationService.DeleteLocation(location); // Call the DeleteLocation method with the Location object
            if (!success)
            {
                return NotFound("Location not found."); // If no rows are affected, return not found
            }

            return NoContent(); // Successful deletion
        }




        //// DELETE: api/Location/{cityID}
        //[HttpDelete("{cityID}")]
        //public IActionResult DeleteLocation(int cityID)
        //{
        //    bool success = _locationService.DeleteLocation(cityID);
        //    if (!success)
        //    {
        //        return NotFound("Location not found.");
        //    }

        //    return NoContent(); // Successful deletion
        //}
    }

}

