using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            if (!request.Headers.Contains("zip") || !request.Headers.Contains("countryCode"))
            {
                return BadRequest("'zip' and 'countryCode' parameters are required to search by zip code. 'dist' and 'units' are optional.");
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
            catch (SqlException exception)
            {
                // If it's specifically the "Zip code not found error"
                if (exception.Number == 51000)
                {
                    return BadRequest("Provided postal code could not be found.");
                }

                return InternalServerError(exception);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Cleans up and reformats postal code for proper db matching
        /// </summary>
        /// <param name="postalCode">The postal code to be searched with</param>
        /// <param name="countryCode">The target country's code</param>
        /// <returns>Reformatted, upper-cased string</returns>
        public string PreparePostalCode(string postalCode, string countryCode)
        {
            if (String.IsNullOrEmpty(postalCode) || String.IsNullOrEmpty(countryCode))
            {
                return null;
            }
            postalCode = postalCode.TrimStart(' ').TrimEnd(' ');
            switch (countryCode)
            {
                // US, in case they include extended zip
                case "US":
                    if (postalCode.Length > 5)
                    {
                        postalCode = postalCode.Substring(0, 5);
                    }
                    break;
                
                // Countries only including first three characters
                case "CA": // Canada
                case "MT": // Malta
                case "IE": // Ireland
                    if (postalCode.Length > 3)
                    {
                        postalCode = postalCode.Substring(0, 3);
                    }
                    break;
                
                // Chile: e.g. 2340000
                case "CL":
                    if (postalCode.Length >= 3)
                    {
                        postalCode = $"{postalCode.Substring(0, 3)}0000";
                    }

                    break;
                    
                // Argentina
                case "AR":
                    if (postalCode.Length > 4)
                    {
                        postalCode = postalCode.Substring(0, 4);
                    }

                    break;

                // Brazil: Only -000 variants of the zip code are listed in the db
                case "BR":
                    if (postalCode.Length >= 5)
                    {
                        postalCode = $"{postalCode.Substring(0, 5)}-000";
                    }

                    break;
                
                // Great Britain: For copyright reasons, only outward (first 3-4 characters) are in the db.
                case "GB":
                    var outwardCodeSeparationIndex = postalCode.IndexOf(' ');
                    if (outwardCodeSeparationIndex >= 0)
                    {
                        postalCode = $"{postalCode.Substring(0, outwardCodeSeparationIndex)}";
                    }
                    else if (postalCode.Length > 4)
                    {
                        postalCode = postalCode.Substring(0, 4);
                    }
                    break;
            }

            return postalCode.ToUpper();
        }
        
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
        public void ValidateParameters(int range, string unit)
        {
            StringBuilder sb = new StringBuilder("Invalid Location Search Parameters:");
            var valid = true;

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