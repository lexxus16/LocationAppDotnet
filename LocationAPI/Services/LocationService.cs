using System.Collections.Generic;
using System.Data.SqlClient;
//using LocationAPI.Models;
using System.Linq;
using System.Text;
using LocationAPI.Models;
using LocationAPI.Services;
using Microsoft.Data.SqlClient;

namespace LocationAPI.Services
{
    public class LocationService : ILocationService
    {
        private readonly string _connectionString = "Server=DESKTOP-01SOFCO\\SQLEXPRESS; Database=StudentDB; Integrated Security=True; TrustServerCertificate=True;";

        // Method to get locations, possibly filtered by country
        public IEnumerable<Location> GetLocations(string country)
        {
            List<Location> locations = new List<Location>();
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT c.CountryName, ci.CityName, ci.PostalCode, ci.Province, ci.CityID");
            query.AppendLine("FROM Country c ");
            query.AppendLine("JOIN City ci ON c.CountryID = ci.CountryID");

            if (!string.IsNullOrEmpty(country))
            {
                query.AppendLine("WHERE c.CountryName LIKE @Country");
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query.ToString(), conn);

                if (!string.IsNullOrEmpty(country))
                {
                    cmd.Parameters.AddWithValue("@Country", "%" + country + "%");
                }

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Location location = new Location
                        {
                            CityID = reader["CityID"] != DBNull.Value ? Convert.ToInt32(reader["CityID"]) : 0,  // Safely handle DBNull
                            Country = reader["CountryName"].ToString(),
                            City = reader["CityName"].ToString(),
                            PostalCode = reader["PostalCode"].ToString(),
                            Province = reader["Province"].ToString()
                        };

                        locations.Add(location);
                    }
                }
            }

            return locations;
        }

        // Method to add a new location to the database
        public Location AddLocation(Location newLocation)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Get the CountryID based on the Country Name
                string countryQuery = "SELECT CountryID FROM Country WHERE CountryName = @CountryName";
                SqlCommand countryCmd = new SqlCommand(countryQuery, conn);
                countryCmd.Parameters.AddWithValue("@CountryName", newLocation.Country);
                conn.Open();
                int countryID = (int)countryCmd.ExecuteScalar();

                // Insert new city into the City table
                string query = "INSERT INTO City (CityName, PostalCode, Province, CountryID) OUTPUT INSERTED.CityID VALUES (@CityName, @PostalCode, @Province, @CountryID)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@CityName", newLocation.City);
                cmd.Parameters.AddWithValue("@PostalCode", newLocation.PostalCode);
                cmd.Parameters.AddWithValue("@Province", newLocation.Province);
                cmd.Parameters.AddWithValue("@CountryID", countryID);

                int cityID = (int)cmd.ExecuteScalar(); // Get the newly inserted CityID
                newLocation.CityID = cityID; // Set the CityID on the Location object

                return newLocation;
            }
        }


        public bool UpdateLocation(int cityID, Location updatedLocation, int countryID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE City SET PostalCode = @PostalCode, Province = @Province, CountryID = @CountryID WHERE CityID = @CityID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@CityID", updatedLocation.CityID);
                cmd.Parameters.AddWithValue("@PostalCode", updatedLocation.PostalCode);
                cmd.Parameters.AddWithValue("@Province", updatedLocation.Province);
                cmd.Parameters.AddWithValue("@CountryID", countryID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public int GetCountryID(string countryName)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string countryQuery = "SELECT CountryID FROM Country WHERE CountryName = @CountryName";
                SqlCommand countryCmd = new SqlCommand(countryQuery, conn);
                countryCmd.Parameters.AddWithValue("@CountryName", countryName);

                conn.Open();
                object result = countryCmd.ExecuteScalar();

                return result == DBNull.Value ? 0 : Convert.ToInt32(result); // Returns 0 if no CountryID found
            }
        }



        public bool DeleteLocation(Location location)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Update query to delete based on CityName, PostalCode, and Province
                string query = "DELETE FROM City WHERE CityName = @CityName AND PostalCode = @PostalCode AND Province = @Province";
                SqlCommand cmd = new SqlCommand(query, conn);

                // Add parameters from the Location object
                cmd.Parameters.AddWithValue("@CityName", location.City);
                cmd.Parameters.AddWithValue("@PostalCode", location.PostalCode);
                cmd.Parameters.AddWithValue("@Province", location.Province);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); // Execute the DELETE query

                return rowsAffected > 0; // Return true if a row was deleted
            }
        }





        //public bool DeleteLocation(int cityID)
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        string query = "DELETE FROM City WHERE CityID = @CityID";
        //        SqlCommand cmd = new SqlCommand(query, conn);

        //        cmd.Parameters.AddWithValue("@CityID", cityID);

        //        conn.Open();
        //        int rowsAffected = cmd.ExecuteNonQuery();

        //        return rowsAffected > 0;
        //    }
        //}

    }
}
