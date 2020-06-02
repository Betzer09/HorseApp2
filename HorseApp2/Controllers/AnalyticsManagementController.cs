using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace HorseApp2.Controllers
{
    /// <summary>
    /// API Controller handling analytics requests such as incrementing/decrementing times a listing has been added to
    /// favorites and incrementing listing views
    /// </summary>
    [Route("api/[controller]")]
    public class AnalyticsManagementController : ApiController
    {
        #region Class Variables

        #endregion

        #region Get Requests

        #endregion

        #region Post Requests

        /// <summary>
        /// Increments the view count for the given active listing ID
        /// </summary>
        /// <param name="requestBody">Request pulled from the post body, including the active listing ID</param>
        /// <returns>The number of views, or an error response</returns>
        [HttpPost]
        [Route("IncrementListingViewCount")]
        public async Task<IHttpActionResult> IncrementListingViewCount(ActiveListingIdRequest requestBody)
        {
            if (requestBody == null || string.IsNullOrEmpty(requestBody.ActiveListingId))
            {
                return BadRequest("'activeListingId' is a required parameter.");
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_IncrementActiveListingViewedCount");

                    var dbHelper = new DatabaseHelper();
                    var activeListingIdParameter =
                        dbHelper.BuildSqlParameter("@ActiveListingId", requestBody.ActiveListingId);

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;
                    cmd.Parameters.Add(activeListingIdParameter);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return Content(HttpStatusCode.NotFound,
                            $"Active Listing with ID '{requestBody.ActiveListingId}' could not be found.");
                    }

                    var result = ds.Tables[0].Rows[0].Field<int>("ViewedCount");

                    //close connection and return the count
                    context.Database.Connection.Close();
                    return Ok(new ViewCountResponse {ViewedCount = result});
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Updates the favorite count for the given active listing ID
        /// </summary>
        /// <param name="requestBody">Request pulled from the post body, including the active listing ID and increment flag</param>
        /// <returns>The number of times the listing has been added to users' favorites, or an error response</returns>
        [HttpPost]
        [Route("UpdateActiveListingFavoritesCount")]
        public async Task<IHttpActionResult> UpdateActiveListingFavoritesCount(ActiveListingIdRequest requestBody)
        {
            if (requestBody == null || string.IsNullOrEmpty(requestBody.ActiveListingId))
            {
                return BadRequest("'activeListingId' is a required parameter.");
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_UpdateActiveListingFavoritesCount");

                    var dbHelper = new DatabaseHelper();
                    var activeListingIdParameter =
                        dbHelper.BuildSqlParameter("@ActiveListingId", requestBody.ActiveListingId);
                    var incrementParameter =
                        dbHelper.BuildSqlParameter("@Incrementing", requestBody.IsIncrementing);


                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;
                    cmd.Parameters.Add(activeListingIdParameter);
                    cmd.Parameters.Add(incrementParameter);

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    {
                        return Content(HttpStatusCode.NotFound,
                            $"Active Listing with ID '{requestBody.ActiveListingId}' could not be found.");
                    }

                    var result = ds.Tables[0].Rows[0].Field<int>("FavoriteCount");

                    //close connection and return the count
                    context.Database.Connection.Close();
                    return Ok(new FavoritesCountResponse {FavoriteCount = result});
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        #endregion

        #region Helper Functions

        #endregion
    }

    public class ActiveListingIdRequest
    {
        /// <summary>
        /// ID of the requested listing
        /// </summary>
        [JsonProperty("activeListingId")] public string ActiveListingId { get; set; }

        /// <summary>
        /// Flag determining whether favorite adjustments will increment or decrement
        /// </summary>
        [JsonProperty("isIncrementing")] public bool IsIncrementing { get; set; } = true;
    }

    public class ViewCountResponse
    {
        /// <summary>
        /// Count of views for the requested listing
        /// </summary>
        [JsonProperty("viewedCount")] public int ViewedCount { get; set; }
    }

    public class FavoritesCountResponse
    {
        /// <summary>
        /// Count of favorites the requested listing has received
        /// </summary>
        [JsonProperty("favoriteCount")] public int FavoriteCount { get; set; }
    }
}