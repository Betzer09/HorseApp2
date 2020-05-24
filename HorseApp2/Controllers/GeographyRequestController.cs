using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HorseApp2.Models;

namespace HorseApp2.Controllers
{
    /// <summary>
    /// API Controller handling geography-based such as zip code search
    /// </summary>
    [Route("api/[controller]")]
    public class GeographyRequestController : ApiController
    {

        #region Class Variables
        
        /// <summary>
        /// List of zip code options
        /// </summary>
        private readonly string[] _supportedZipCodes =
        {
            "US",
            "CA"
        };
        
        /// <summary>
        /// List of unit type options
        /// </summary>
        private readonly string[] _supportedUnitTypes =
        {
            "MILE",
            "KM"
        };

        /// <summary>
        /// Minimum accepted value for range
        /// </summary>
        private readonly int _minRange = 0;
        
        /// <summary>
        /// Maximum accepted value for range
        /// </summary>
        private readonly int _maxRange = 250;
        
        #endregion
        
        #region Get Requests

        /// <summary>
        /// Search for horses within a provided range of a given zip code.
        /// </summary>
        /// <remarks>Kept active to provide easy access for validation and testing</remarks>
        /// <returns>HTTP response, which includes the search results on success</returns>
        [HttpGet]
        [Route("SearchByZip")]
        public async Task<IHttpActionResult> GetListingsInZipCodeRange()
        {
            var request = Request;
            if (!request.Headers.Contains("zip"))
            {
                return BadRequest("'zip' parameter is required to search by zip code. 'dist' and 'units' are optional.");
            }
            
            var dbHelper = new DatabaseHelper();
            var objRequest = new SearchActiveListingsRequest();
            try
            {
                objRequest = await dbHelper.BuildListingRequest(request.Headers);
            }
            catch (HttpRequestException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpException exception)
            {
                return InternalServerError(exception);
            }

            if (!IsValidZipCode(objRequest.PostalCode))
            {
                return BadRequest("Invalid postal code format provided.");
            }

            if (!IsSupportedCountryCode(objRequest.CountryCode))
            {
                return BadRequest($"Invalid country code. {objRequest.CountryCode} is not supported.");
            }

            // Connect to the db and search for relevant listings
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    // Initialize command
                    var searchCmd = new SqlCommand("usp_SearchActiveListings");
                    searchCmd.CommandType = CommandType.StoredProcedure;

                    // Initialize Parameters
                    var parameters = dbHelper.GetSqlParametersForSearchListings(objRequest);

                    // Set up connection
                    searchCmd.Connection = new SqlConnection(context.Database.Connection.ConnectionString);
                    searchCmd.Parameters.AddRange(parameters.ToArray());
                    context.Database.Connection.Open();
                    var adapter = new SqlDataAdapter(searchCmd);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    var listingData = dataSet.Tables[0];
                    var photos = dataSet.Tables[1];
                    var totalListings = dataSet.Tables[2];

                    int total = 0;
                    foreach (DataRow row in totalListings.Rows)
                    {
                        total = int.Parse(row[0].ToString());
                    }

                    //convert data from stored proceduure into ActiveListing object
                    var response = new SearchResponse();
                    response.listings = dbHelper.DataTablesToHorseListing(listingData, photos);
                    response.totalNumOfListings = total;
                    response.pageNumber = objRequest.Page;

                    //close connection and return
                    context.Database.Connection.Close();
                    return Ok(response);
                }
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        #endregion

        #region Helper Functions
        

        #endregion

        
        #region Validation Functions

        /// <summary>
        /// Validates provided parameters
        /// </summary>
        /// <param name="postalCode">Postal code being searched for</param>
        /// <param name="countryCode">2 character code for the country the postal code belongs to</param>
        /// <param name="range">Distance (radius) results should be limited to</param>
        /// <param name="unit">Unit type for the distance</param>
        /// <exception cref="Exception">List of invalid parameters</exception>
        public void ValidateParameters(string postalCode, string countryCode, int range, string unit)
        {
            StringBuilder sb = new StringBuilder("Invalid Location Search Parameters:");
            var valid = true;
            if (!IsValidZipCode(postalCode))
            {
                valid = false;
                sb.Append(" Postal code format not recognized.");
            }

            if (!IsSupportedCountryCode(countryCode))
            {
                valid = false;
                sb.Append(" Country code not supported.");
            }

            if (!IsValidRange(range))
            {
                valid = false;
                sb.Append($" Range must be between {_minRange.ToString()} and {_maxRange.ToString()} inclusive.");
            }

            if (!IsSupportedUnitType(unit))
            {
                valid = false;
                sb.Append(" Unit type not recognized.");
            }

            if (!valid)
            {
                throw new Exception(sb.ToString());
            }
        }
        
        /// <summary>
        /// Checks to see if the provided input is a valid zip code.
        /// </summary>
        /// <param name="input">Zip code to check</param>
        /// <returns>Validity of the zip code</returns>
        public bool IsValidZipCode(string input)
        {
            var regexes = new []
            {
                // US Zip: e.g. 83402
                new Regex(@"^\d{5}(?:[-\s]\d{4})?$"), 
                // UK Zip: e.g. CW3 9SS
                new Regex(@"^[A-Z]{1,2}\d[A-Z\d]? ?\d[A-Z]{2}$"),
                // CA Zip: e.g. M1R 0E9
                new Regex(@"[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]"),
            };

            return regexes.Any(regex => regex.IsMatch(input));
        }

        /// <summary>
        /// Checks to see if country code is accepted
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public bool IsSupportedCountryCode(string countryCode)
        {
            return _supportedZipCodes.Contains(countryCode.ToUpper());
        }

        /// <summary>
        /// Checks to see if provided range is within constraints
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool IsValidRange(int range)
        {
            return _minRange <= range  && range <= _maxRange;
        }

        /// <summary>
        /// Checks to see if provided unit type is accepted
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool IsSupportedUnitType(string unit)
        {
            return _supportedUnitTypes.Contains(unit.ToUpper());
        }

        #endregion
    }
}