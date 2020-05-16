using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HorseApp2.Models;
using HorseApp2.Models.Geography;

namespace HorseApp2.Controllers
{
    /// <summary>
    /// API Controller handling geography-based such as zip code search, 
    /// </summary>
    [Route("api/[controller]")]
    public class GeographyRequestController : ApiController
    {
        #region Get Requests

        /// <summary>
        /// Search for horses within a provided range of a given zip code.
        /// </summary>
        /// <param name="requestDto">Zip code request object used for search</param>
        /// <returns>HTTP response, which includes the search results on success</returns>
        [HttpGet]
        [Route("ListingsInRange")]
        public async Task<IHttpActionResult> GetListingsInZipCodeRange([FromUri] ZipCodeSearchRequestDTO requestDto)
        {
            var dtoResults = new List<ZipCodeSearchResult>();
            try
            {
                var requestResults = await FetchZipCodesInRange(requestDto);
                dtoResults.AddRange(requestResults);
            }
            catch (HttpRequestException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (HttpException exception)
            {
                return InternalServerError(new HttpException(exception.Message));
            }

            // If no zip codes are found, then there will be no results so we can skip the db query
            if (dtoResults.Count <= 0)
            {
                var horseResults = new HorseListing[] { };
                return Ok(horseResults);
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
                    var dbHelper = new DatabaseHelper();
                    // TODO: Finish param setup
                    var objRequest = new SearchActiveListingsRequest();
                    var zipCodes = new List<string>();
                    foreach (var result in dtoResults)
                    {
                        zipCodes.Add(result.ZipCode);
                    }

                    // objRequest.Locations = zipCodes;
                    // objRequest.LocationsSearch = true;
                    
                    var parameters = dbHelper.GetSqlParametersForSearchListings(objRequest);

                    // Set up connection
                    searchCmd.Connection = new SqlConnection(context.Database.Connection.ConnectionString);
                    foreach (var param in parameters)
                    {
                        searchCmd.Parameters.Add(param);
                    }

                    context.Database.Connection.Open();
                    var adapter = new SqlDataAdapter(searchCmd);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    var listingData = dataSet.Tables[0];
                    var photos = dataSet.Tables[1];

                    // Convert result data to Active Listing

                    var results = dbHelper.DataTablesToHorseListing(listingData, photos);

                    // Cleanup and Return
                    context.Database.Connection.Close();
                    return Ok(results);
                }
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Retrieves all zip codes within the provided range of the given zip code
        /// </summary>
        /// <param name="requestDto">Zip code request object used for search</param>
        /// <returns>Array of relevant zip codes and their associated data</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="HttpException"></exception>
        private async Task<ZipCodeSearchResult[]> FetchZipCodesInRange(ZipCodeSearchRequestDTO requestDto)
        {
            var request = new ZipCodeSearchRequest()
            {
                OriginZipCode = requestDto.zip,
                Distance = requestDto.dist,
                Units = requestDto.units
            };

            // Pre-flight checks before the API is called
            if (!IsZipCode(request.OriginZipCode))
            {
                throw new HttpRequestException("Invalid Zip Code");
                // return BadRequest("Invalid Zip Code");
            }

            if (request.Units != "mile" && request.Units != "km")
            {
                throw new HttpRequestException("Invalid units parameter. Accepted units are 'mile' or 'km'");
            }

            // Build API Request
            var client = new HttpClient();
            var baseUrl = "https://www.zipcodeapi.com/rest";
            // TODO: Replace this with the company one when I'm done
            var authToken = "0rVJ830UQmMfUSFq8aatxDldCp9LOwVpTnjWgXpblrEtcQSTkmrYjQHc2dm2yyiB";
            var endpoint = "radius.json";
            var completeUrl =
                $"{baseUrl}/{authToken}/{endpoint}/{request.OriginZipCode}/{request.Distance.ToString()}/{request.Units}";

            // Parse the API call result
            var result = await client.GetAsync(completeUrl);

            // Handle any non-successful response codes from the API
            if (!result.IsSuccessStatusCode)
            {
                switch (result.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new HttpException("Malformed API Request: 400");
                    case HttpStatusCode.Unauthorized:
                        throw new HttpException("Unauthorized API access attempt: 401");
                    case HttpStatusCode.NotFound:
                        throw new HttpException("Provided zip code could not be found: 404");
                }

                // Has to be checked separately since the code isn't included in the HttpStatusCode enum
                if (result.StatusCode.GetHashCode() == 429)
                {
                    throw new HttpException("Allowed API usage has been exceeded: 429");
                }
            }

            // Parse and return the zip code results from the API
            var data = await result.Content.ReadAsAsync<ZipCodeSearchRequestResultsDTO>();
            var zipCodeResults = new List<ZipCodeSearchResult>();
            foreach (var dto in data.zip_codes)
            {
                zipCodeResults.Add(new ZipCodeSearchResult(dto));
            }

            return zipCodeResults.ToArray();
        }

        #endregion

        
        #region Validation Functions
        
        /// <summary>
        /// Checks to see if the provided input is a valid zip code.
        /// </summary>
        /// <param name="input">Zip code to check</param>
        /// <returns>Validity of the zip code</returns>
        private bool IsZipCode(string input)
        {
            Regex regex = new Regex(@"^\d{5}(?:[-\s]\d{4})?$");
            return regex.IsMatch(input);
        }

        #endregion
    }
}