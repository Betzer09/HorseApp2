using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using HorseApp2.Models;

namespace HorseApp2.Controllers
{
    /// <summary>
    /// API Controller handling analytics requests such as incrementing/decrementing times a listing has been added to
    /// favorites and incrementing listing views
    /// </summary>
    [Route("api/[controller]")]
    public class AnalyticsManagementController: ApiController
    {
        #region Class Variables

        

        #endregion

        #region Get Requests

        

        #endregion

        #region Post Requests
        
        // Add View
        /// <summary>
        /// Search for horses within a provided range of a given zip code.
        /// </summary>
        /// <remarks>Kept active to provide easy access for validation and testing</remarks>
        /// <returns>HTTP response, which includes the search results on success</returns>
        [HttpPost]
        [Route("IncrementListingViewCount")]
        public async Task<IHttpActionResult> IncrementListingViewCount([FromBody] string activeListingId)
        {
            var request = Request;
            if (String.IsNullOrEmpty(activeListingId))
            {
                return BadRequest("'activeListingId' is a required parameter.");
            }

            return Ok();
            // try
            // {
            //     using (var context = new HorseDatabaseEntities())
            //     {
            //         //Initializing sql command, parameters, and connection
            //         SqlCommand cmd = new SqlCommand("usp_IncrementListingViewCount");
            //         
            //         var dbHelper = new DatabaseHelper();
            //         var activeListingId = dbHelper.BuildSqlParameter("@ActiveListingId", )
            //         
            //         cmd.CommandType = CommandType.StoredProcedure;
            //         SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
            //         cmd.Connection = conn;
            //         cmd.Parameters.AddRange(parameters.ToArray());
            //
            //
            //         //open connection
            //         context.Database.Connection.Open();
            //
            //         //execute and retrieve data from stored procedure
            //         SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //         DataSet ds = new DataSet();
            //         adapter.Fill(ds);
            //         DataTable listingData = ds.Tables[0];
            //         DataTable photos = ds.Tables[1];
            //         DataTable totalListings = ds.Tables[2];
            //
            //         int total = 0;
            //         foreach (DataRow row in totalListings.Rows)
            //         {
            //             total = int.Parse(row[0].ToString());
            //         }
            //
            //         //convert data from stored proceduure into ActiveListing object
            //         SearchResponse response = new SearchResponse();
            //         response.listings = dbHelper.DataTablesToHorseListing(listingData, photos);
            //         response.totalNumOfListings = total;
            //         response.pageNumber = objRequest.Page;
            //
            //         //close connection and return
            //         context.Database.Connection.Close();
            //         return Ok(response);
            //     }
            // }
            // catch (SqlException exception)
            // {
            //     // If it's specifically the "Zip code not found error"
            //     if (exception.Number == 51000)
            //     {
            //         var contentResult = new NegotiatedContentResult<ResponseMessage>(
            //             HttpStatusCode.NoContent, 
            //             new ResponseMessage {Message = "Provided postal code could not be found."},
            //             this);
            //         return contentResult;
            //     }
            //
            //     return InternalServerError(exception);
            // }
            // catch (Exception e)
            // {
            //     return InternalServerError(e);
            // }
        }
        
        // Adjust Favorites Count

        #endregion

        #region Helper Functions

        

        #endregion
    }
}