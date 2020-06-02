using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using HorseApp2.Versions.v1_0.Models;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_0.Controllers
{
    //Main Controller
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class HorseController : ApiController
    {
        //Test Endpoint to see if connection to the database is successful
        //Enter a row number in the header and it will return true if the row exists in tblName
        [HttpGet]
        [Route("RowExists")]
        public bool Exists()
        {
            int row = 0;
            var request = this.Request;
            var headers = request.Headers;

            if (headers.Contains("row")) ;
            {
                row = int.Parse(headers.GetValues("row").FirstOrDefault());
            }


            int exists = 0;
            string storedProcedureName = "usp_RowExists";
            string parameterSequence = "@row";

            SqlParameter[] sqlParameter =
            {
                new SqlParameter {ParameterName = "@row", SqlDbType = SqlDbType.BigInt, Value = row},
            };

            using (var context = new HorseDatabaseEntities())
            {
                exists = context.Database.SqlQuery<int>(storedProcedureName + " " + parameterSequence, sqlParameter)
                    .FirstOrDefault();
            }

            if (exists > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts an Active Listing into tblActiveListing
        /// TESTED AND WORKING
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertActiveListing")]
        public HorseListing InsertActiveListing(HorseListing listing)
        {
            //response object
            HorseListing response = new HorseListing();
            var dbHelper = new DatabaseHelper();
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_InsertActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = dbHelper.GetSqlParametersForInsert(listing);
                    SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    foreach (SqlParameter param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    //DataTable photos = ds.Tables[1];
                    DataTable photos = new DataTable();

                    var listings = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //convert data from stored proceduure into ActiveListing object
                    if (listings.Count() > 0)
                    {
                        response = listings.ElementAt(0);
                    }
                    else
                    {
                        response = new HorseListing();
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
                response = new HorseListing();
            }

            return response;
        }

        /// <summary>
        /// Updates an active listing given a particular ActiveListingId and a field to update
        /// All fields are optional
        /// This is also the proper endpoint where inserting photos should occur
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateActiveListing")]
        public HorseListing UpdateActiveListing(HorseListing listing)
        {
            //Create and format Sql Parameters for stored procedure
            List<SqlParameter> parameters = new List<SqlParameter>();
            var dbHelper = new DatabaseHelper();

            SqlParameter param0 = new SqlParameter();
            param0.ParameterName = "@ActiveListingId";
            param0.Value = listing.activeListingId;
            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@Age";
            param1.Value = listing.age;
            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@Color";
            param2.Value = listing.color;
            SqlParameter param3 = new SqlParameter();
            param3.ParameterName = "@Dam";
            param3.Value = listing.dam;
            SqlParameter param5 = new SqlParameter();
            param5.ParameterName = "@Sire";
            param5.Value = listing.sire;
            SqlParameter param6 = new SqlParameter();
            param6.ParameterName = "@DamSire";
            param6.Value = listing.damSire;
            SqlParameter param7 = new SqlParameter();
            param7.ParameterName = "@Description";
            param7.Value = listing.description;
            SqlParameter param9 = new SqlParameter();
            param9.ParameterName = "@Gender";
            param9.Value = listing.gender;
            SqlParameter param10 = new SqlParameter();
            param10.ParameterName = "@HorseName";
            param10.Value = listing.horseName;
            SqlParameter param11 = new SqlParameter();
            param11.ParameterName = "@InFoal";
            param11.Value = listing.inFoal;
            SqlParameter param12 = new SqlParameter();
            param12.ParameterName = "@Lte";
            param12.Value = listing.lte;
            SqlParameter param13 = new SqlParameter();
            param13.ParameterName = "@OriginalDateListed";
            param13.Value = listing.originalDateListed;
            SqlParameter param14 = new SqlParameter();
            param14.ParameterName = "@Price";
            param14.Value = listing.price;
            SqlParameter param15 = new SqlParameter();
            param15.ParameterName = "@PurchaseListingType";
            param15.Value = listing.purchaseListingType;
            SqlParameter param16 = new SqlParameter();
            param16.ParameterName = "@RanchPhoto";
            param16.Value = listing.ranchPhoto;
            SqlParameter param17 = new SqlParameter();
            param17.ParameterName = "@SellerId";
            param17.Value = listing.sellerId;
            SqlParameter param18 = new SqlParameter();
            param18.ParameterName = "@HorseType";
            param18.Value = listing.horseType;
            SqlParameter param19 = new SqlParameter();
            param19.ParameterName = "@IsSold";
            param19.Value = listing.isSold;
            SqlParameter param20 = new SqlParameter();
            param20.ParameterName = "@InFoalTo";
            param20.Value = listing.InFoalTo;
            SqlParameter param22 = new SqlParameter();
            param22.ParameterName = "@CallForPrice";
            param22.Value = listing.callForPrice;
            SqlParameter param23 = new SqlParameter();
            param23.ParameterName = "@Height";
            param23.Value = listing.Height;
            var geographyHelper = new GeographyRequestController();
            SqlParameter param24 = new SqlParameter();
            param24.ParameterName = "@Zip";
            param24.Value = geographyHelper.PreparePostalCode(listing.Zip, listing.CountryCode);
            SqlParameter param25 = new SqlParameter();
            param25.ParameterName = "@CountryCode";
            param25.Value = listing.CountryCode;
            SqlParameter param26 = new SqlParameter();
            param26.ParameterName = "@FavoriteCount";
            param26.Value = listing.FavoriteCount;
            SqlParameter param27 = new SqlParameter();
            param27.ParameterName = "@ViewedCount";
            param27.Value = listing.ViewedCount;


            /*
            SqlParameter param24 = new SqlParameter();
            param24.ParameterName = "@IsSireRegistered";
            param24.Value = listing.IsSireRegistered;

            SqlParameter param25 = new SqlParameter();
            param25.ParameterName = "@IsDamSireRegistered";
            param25.Value = listing.IsDamSireRegistered;
            */


            SqlParameter photos = new SqlParameter();
            photos.ParameterName = "@Photos";


            DataTable dt = new DataTable();
            DataColumn ActiveListingPhotoIdColumn = new DataColumn("ActiveListingPhotoId");
            ActiveListingPhotoIdColumn.DataType = System.Type.GetType("System.String");
            DataColumn ActiveListingIdColumn = new DataColumn("ActiveListingId");
            ActiveListingIdColumn.DataType = System.Type.GetType("System.String");
            DataColumn PhotoURLColumn = new DataColumn("PhotoURL");
            PhotoURLColumn.DataType = System.Type.GetType("System.String");
            DataColumn PhotoOrderColumn = new DataColumn("PhotoOrder");
            PhotoOrderColumn.DataType = System.Type.GetType("System.Int32");
            DataColumn CreatedOnColumn = new DataColumn("CreatedOn");
            CreatedOnColumn.DataType = System.Type.GetType("System.DateTime");
            DataColumn UpdatedOnColumn = new DataColumn("UpdatedOn");
            UpdatedOnColumn.DataType = System.Type.GetType("System.DateTime");
            DataColumn IsVideoColumn = new DataColumn("IsVideo");
            IsVideoColumn.DataType = System.Type.GetType("System.Boolean");
            dt.Columns.Add(ActiveListingPhotoIdColumn);
            dt.Columns.Add(ActiveListingIdColumn);
            dt.Columns.Add(PhotoURLColumn);
            dt.Columns.Add(PhotoOrderColumn);
            dt.Columns.Add(CreatedOnColumn);
            dt.Columns.Add(UpdatedOnColumn);
            dt.Columns.Add(IsVideoColumn);

            List<DataRow> rows = new List<DataRow>();
            int rowCount = listing.photos.Count();
            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(dt.NewRow());
            }

            int j = 0;
            HorseListingPhoto photo;
            foreach (DataRow row in rows)
            {
                photo = listing.photos.ElementAt(j);
                row["ActiveListingPhotoId"] = photo.activeListingPhotoId;
                row["ActiveListingId"] = photo.activeListingId;
                row["PhotoURL"] = photo.photoUrl;
                row["PhotoOrder"] = photo.photoOrder;
                row["CreatedOn"] = photo.createdOn;
                row["UpdatedOn"] = DateTime.Now.ToString();
                row["IsVideo"] = photo.isVideo;


                dt.Rows.Add(row);

                j++;
            }

            photos.Value = dt;


            parameters.Add(param0);
            parameters.Add(param1);
            parameters.Add(param2);
            parameters.Add(param3);
            parameters.Add(param5);
            parameters.Add(param6);
            parameters.Add(param7);
            parameters.Add(param9);
            parameters.Add(param10);
            parameters.Add(param11);
            parameters.Add(param12);
            parameters.Add(param13);
            parameters.Add(param14);
            parameters.Add(param15);
            parameters.Add(param16);
            parameters.Add(param17);
            parameters.Add(param18);
            parameters.Add(param19);
            parameters.Add(param20);
            parameters.Add(param22);
            parameters.Add(param23);
            parameters.Add(param24);
            parameters.Add(param25);
            parameters.Add(param26);
            parameters.Add(param27);
            parameters.Add(photos);


            //response object
            HorseListing response = new HorseListing();
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_UpdateActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;

                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    foreach (SqlParameter param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photosFromDb = ds.Tables[1];


                    var listings = dbHelper.DataTablesToHorseListing(listingData, photosFromDb);

                    //convert data from stored proceduure into ActiveListing object
                    if (listings.Count() > 0)
                    {
                        response = listings.ElementAt(0);
                    }
                    else
                    {
                        response = new HorseListing();
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
                response = new HorseListing();
            }

            return response;
        }

        /// <summary>
        /// Deletes an ActiveListing or multiple ActiveListings given activeListingIds
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteActiveListings")]
        public List<HorseListing> DeleteActiveListings()
        {
            List<HorseListing> deleteHorseListings = new List<HorseListing>();
            List<string> ActiveListingIds = new List<string>();
            var dbHelper = new DatabaseHelper();

            var request = this.Request;
            var headers = request.Headers;


            if (headers.Contains("ActiveListingIds"))
            {
                string ids = headers.GetValues("ActiveListingIds").First().Trim(new Char[] {'{', '}', '[', ']'})
                    .Replace("\"", "");

                //ids = ids.ToString();

                ActiveListingIds = ids.ToString().Split(',').ToList();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_DeleteActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@ActiveListingIds";
                    param.Value = ListingIdListToDatatable(ActiveListingIds);
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    deleteHorseListings = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return deleteHorseListings;
        }

        [HttpGet]
        [Route("GetDeletedListings")]
        public List<HorseListing> GetDeletedListings()
        {
            List<HorseListing> response = new List<HorseListing>();
            var dbHelper = new DatabaseHelper();


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_GetDeletedListings");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();

                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches though active listings given filter fields
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchActiveListings")]
        public async Task<IHttpActionResult> SearchActiveListings()
        {
            var request = Request;
            var dbHelper = new DatabaseHelper();
            string name = "";
            SearchResponse response = new SearchResponse();
            string input = "";

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

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchActiveListings");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = dbHelper.GetSqlParametersForSearchListings(objRequest);
                    SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;
                    cmd.Parameters.AddRange(parameters.ToArray());


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];
                    DataTable totalListings = ds.Tables[2];

                    int total = 0;
                    foreach (DataRow row in totalListings.Rows)
                    {
                        total = int.Parse(row[0].ToString());
                    }

                    //convert data from stored proceduure into ActiveListing object
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
                    var contentResult = new NegotiatedContentResult<ResponseMessage>(
                        HttpStatusCode.NoContent, 
                        new ResponseMessage {Message = "Provided postal code could not be found."},
                        this);
                    return contentResult;
                }

                return InternalServerError(exception);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Inserts a sire into the sire table
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("InsertSire")]
        public SireResponse InsertSire()
        {
            SireResponse response = new SireResponse();

            var request = this.Request;
            var headers = request.Headers;
            string name = "";
            string horseType = "";

            if (headers.Contains("Name"))
            {
                name = headers.GetValues("Name").First().ToString();
            }

            if (headers.Contains("HorseType"))
            {
                horseType = headers.GetValues("horseType").First().ToString();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_InsertSire");
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@HorseType";
                    param2.Value = horseType;
                    cmd.Parameters.Add(param2);

                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData;
                    if (ds.Tables[0] != null)
                    {
                        SireData = ds.Tables[0];
                    }
                    else
                    {
                        SireData = new DataTable();
                    }

                    //convert data from stored procedure into Sire object
                    if (SireTableToSireResponse(SireData).Count() > 0)
                    {
                        response = SireTableToSireResponse(SireData).ElementAt(0);
                    }
                    else
                    {
                        response = new SireResponse();
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        [HttpPut]
        [Route("UpdateSireName")]
        public SireResponse UpdateSireName(UpdateSireNameRequest objRequest)
        {
            SireResponse response = new SireResponse();


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_UpdateSireName");
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@oldName";
                    param.Value = objRequest.oldName;
                    cmd.Parameters.Add(param);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@newName";
                    param2.Value = objRequest.newName;
                    cmd.Parameters.Add(param2);

                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData;
                    if (ds.Tables[0] != null)
                    {
                        SireData = ds.Tables[0];
                    }
                    else
                    {
                        SireData = new DataTable();
                    }

                    //convert data from stored procedure into Sire object
                    if (SireTableToSireResponse(SireData).Count() > 0)
                    {
                        response = SireTableToSireResponse(SireData).ElementAt(0);
                    }
                    else
                    {
                        response = new SireResponse();
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                response = null;
            }


            return response;
        }

        /// <summary>
        /// Deletes a sire
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSire")]
        public SireResponse DeleteSire()
        {
            SireResponse response = new SireResponse();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";

            if (headers.Contains("Name"))
            {
                name = headers.GetValues("Name").First();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_DeleteSire");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData = ds.Tables[0];

                    //convert data from stored procedure into Sire object
                    if (SireTableToSireResponse(SireData).Count() > 0)
                    {
                        response = SireTableToSireResponse(SireData).ElementAt(0);
                    }
                    else
                    {
                        response = new SireResponse();
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches all sires given a name
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchAllSires")]
        public List<List<SireResponse>> SearchAllSires()
        {
            List<List<SireResponse>> response = new List<List<SireResponse>>();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";

            if (headers.Contains("Name"))
            {
                name = headers.GetValues("Name").First();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchAllSires");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData = ds.Tables[0];

                    List<SireResponse> listFromDB = SireTableToSireResponse(SireData);
                    //convert data from stored procedure into Sire object
                    int i = 0;
                    int j = 0;
                    response[j] = new List<SireResponse>();
                    foreach (SireResponse s in listFromDB)
                    {
                        response[j].Add(s);
                        i++;
                        if (i == 19)
                        {
                            i = 0;
                            j++;
                            response[j] = new List<SireResponse>();
                        }
                    }


                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches all sires that match given a string
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchAllSiresElastically")]
        public SireSearchResponse SearchAllSiresElastically()
        {
            SireSearchResponse response = new SireSearchResponse();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";
            bool nameSearch = false;
            bool horseTypeSearch = false;
            string[] horseTypes = new string[0];
            int page = 0;
            int itemsPerPage = 0;

            if (headers.Contains("Name"))
            {
                nameSearch = true;
                name = headers.GetValues("Name").First();
            }
            else
            {
                nameSearch = false;
            }

            if (headers.Contains("horseTypes"))
            {
                horseTypeSearch = true;
                horseTypes = headers.GetValues("horseTypes").First().Split(' ');
            }
            else
            {
                horseTypeSearch = false;
            }

            if (headers.Contains("page"))
            {
                page = int.Parse(headers.GetValues("page").First());
            }
            else
            {
                page = 1;
            }

            if (headers.Contains("itemsPerPage"))
            {
                itemsPerPage = int.Parse(headers.GetValues("itemsPerPage").First());
            }
            else
            {
                itemsPerPage = 20;
            }


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchAllSiresElastically");
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter param0 = new SqlParameter();
                    param0.ParameterName = "@NameSearch";
                    param0.Value = nameSearch;
                    cmd.Parameters.Add(param0);

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@HorseTypeSearch";
                    param1.Value = horseTypeSearch;
                    cmd.Parameters.Add(param1);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@Name";
                    param2.Value = name;
                    cmd.Parameters.Add(param2);

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@HorseTypes";
                    DataTable dt = new DataTable();
                    DataColumn dc = new DataColumn();
                    dc.ColumnName = "HorseType";
                    dc.DataType = Type.GetType("System.String");
                    dt.Columns.Add(dc);

                    for (int i = 0; i < horseTypes.Length; i++)
                    {
                        DataRow dr = dt.NewRow();
                        if (horseTypes[i] == "cowHorse")
                        {
                            dr[0] = "cow horse";
                        }
                        else
                        {
                            dr[0] = horseTypes[i];
                        }

                        dt.Rows.Add(dr);
                    }

                    param3.Value = dt;
                    cmd.Parameters.Add(param3);

                    SqlParameter param4 = new SqlParameter();
                    param4.ParameterName = "@Page";
                    param4.Value = page;
                    cmd.Parameters.Add(param4);

                    SqlParameter param5 = new SqlParameter();
                    param5.ParameterName = "@itemsPerPage";
                    param5.Value = itemsPerPage;
                    cmd.Parameters.Add(param5);


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData = ds.Tables[0];
                    DataTable count = ds.Tables[1];


                    //convert data from stored procedure into Sire object
                    response = SireTableToSireSearchResponse(SireData, count);

                    //close connection
                    context.Database.Connection.Close();

                    response.pageCount = page;
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }


        /// <summary>
        /// Retrieves all Active Listings within a specified price range
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchByPrice")]
        public List<HorseListing> SearchByPrice()
        {
            //generic values for price range if none are found
            decimal Low = 0;
            decimal High = 1000000;

            var request = this.Request;
            var headers = request.Headers;
            decimal priceLow = 0;
            decimal priceHigh = 1000000;
            SearchActiveListingsRequest objRequest = new SearchActiveListingsRequest();
            List<HorseListing> response = new List<HorseListing>();
            var dbHelper = new DatabaseHelper();


            if (headers.Contains("PriceHigh"))
            {
                priceLow = decimal.Parse(headers.GetValues("PriceLow").First());
            }

            if (headers.Contains("PriceLow"))
            {
                priceHigh = decimal.Parse(headers.GetValues("PriceHigh").First());
            }


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_searchByPrice");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();
                    SqlParameter param2 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    param1.ParameterName = "@Low";
                    param1.Value = priceLow;
                    param2.ParameterName = "@High";
                    param2.Value = priceHigh;


                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }


        /// <summary>
        /// Searches all active listings by HorseType
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchActiveListingByType")]
        public List<HorseListing> SearchListingByType()
        {
            var request = this.Request;
            var headers = request.Headers;

            List<HorseListing> response = new List<HorseListing>();
            var dbHelper = new DatabaseHelper();
            string type = "";


            if (headers.Contains("Type"))
            {
                type = headers.GetValues("Type").First();
            }


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchByHorseType");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    param1.ParameterName = "@Type";
                    param1.Value = type;


                    cmd.Parameters.Add(param1);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches Active Listings by the horses sire
        /// TESTED AND WORKING
        /// </summary>
        [HttpGet]
        [Route("SearchActiveListingsBySire")]
        public List<HorseListing> SearchListingBySire()
        {
            var request = this.Request;
            var headers = request.Headers;
            var dbHelper = new DatabaseHelper();

            List<HorseListing> response = new List<HorseListing>();
            string name = "";


            if (headers.Contains("Name"))
            {
                name = headers.GetValues("Name").First();
            }


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchBySire");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    param1.ParameterName = "@Sire";
                    param1.Value = name;


                    cmd.Parameters.Add(param1);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches Active Listings by gender
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchActiveListingByGender")]
        public List<HorseListing> SearchListingByGender()
        {
            var request = this.Request;
            var headers = request.Headers;
            var dbHelper = new DatabaseHelper();

            List<HorseListing> response = new List<HorseListing>();
            string gender = "";


            if (headers.Contains("Gender"))
            {
                gender = headers.GetValues("Gender").First();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchByGender");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    param1.ParameterName = "@Gender";
                    param1.Value = gender;


                    cmd.Parameters.Add(param1);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        /// <summary>
        /// Searches ActiveListings by Age
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchActiveListingsByAge")]
        public List<HorseListing> SearchListingByAge()
        {
            var request = this.Request;
            var headers = request.Headers;
            var dbHelper = new DatabaseHelper();

            List<HorseListing> response = new List<HorseListing>();
            List<int> ages = new List<int>();


            if (headers.Contains("Ages"))
            {
                string[] agesArray = headers.GetValues("Ages").First().Split(' ');

                for (int i = 0; i < agesArray.Length; i++)
                {
                    ages.Add(int.Parse(agesArray[i]));
                }
            }


            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_v1_0_SearchByAge");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    SqlParameter agesParam = new SqlParameter();
                    agesParam.ParameterName = "@Ages";

                    DataTable dt = new DataTable();
                    DataColumn ageColumn = new DataColumn("Age");

                    dt.Columns.Add(ageColumn);

                    List<DataRow> rows = new List<DataRow>();

                    int rowCount = ages.Count();
                    for (int i = 0; i < rowCount; i++)
                    {
                        rows.Add(dt.NewRow());
                    }

                    int j = 0;
                    foreach (DataRow row in rows)
                    {
                        row["Age"] = ages.ElementAt(j);
                        dt.Rows.Add(row);
                        j++;
                    }

                    agesParam.Value = dt;


                    cmd.Parameters.Add(agesParam);


                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable listingData = ds.Tables[0];
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = dbHelper.DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }

            return response;
        }

        private SireSearchResponse SireTableToSireSearchResponse(DataTable sireTable, DataTable count)
        {
            SireSearchResponse response = new SireSearchResponse();

            foreach (DataRow row in sireTable.Rows)
            {
                response.sires.Add(PopulateSireSearchResponse(row));
            }

            foreach (DataRow row in count.Rows)
            {
                response.totalResultCount = long.Parse(row[0].ToString());
            }

            return response;
        }

        private SireListing PopulateSireSearchResponse(DataRow row)
        {
            SireListing response = new SireListing();

            response.sireServerId = long.Parse(row["SireServerId"].ToString());
            response.name = row["Name"].ToString();
            response.horseType = row["HorseType"].ToString();
            response.createdOn = row["CreatedOn"].ToString();

            return response;
        }


        private List<SireResponse> SireTableToSireResponse(DataTable sireTable)
        {
            List<SireResponse> response = new List<SireResponse>();

            foreach (DataRow row in sireTable.Rows)
            {
                response.Add(PopulateSireResponse(row));
            }

            return response;
        }

        private SireResponse PopulateSireResponse(DataRow row)
        {
            SireResponse response = new SireResponse();

            response.sireServerId = long.Parse(row["SireServerId"].ToString());
            response.name = row["Name"].ToString();
            response.horseType = row["HorseType"].ToString();
            response.createdOn = row["CreatedOn"].ToString();

            return response;
        }


        private DataTable ListingIdListToDatatable(List<string> Ids)
        {
            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("ActiveListingId");
            column.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(column);

            List<DataRow> rows = new List<DataRow>();
            int rowCount = Ids.Count();
            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(dt.NewRow());
            }

            int j = 0;
            long id;
            foreach (DataRow row in rows)
            {
                row["ActiveListingId"] = Ids.ElementAt(j);

                dt.Rows.Add(row);

                j++;
            }

            return dt;
        }


        //If
        //type = 1 ReiningSire
        //tpye = 2 CuttingSire
        //type = 3 CowHorse
        //type = 4 BarrelSire
        [HttpPost]
        [Route("PostSires")]
        public void inputSires()
        {
            int type = -1;

            var headers = this.Request.Headers;

            if (headers.Contains("type"))
            {
                type = int.Parse(headers.GetValues("type").First());
            }


            DataTable dt = new DataTable();
            DataColumn dc1 = new DataColumn();

            dc1.ColumnName = "Name";
            dc1.DataType = Type.GetType("System.String");

            dt.Columns.Add(dc1);


            string horseType = "";
            string[] lines;

            switch (type)
            {
                case 1:
                    lines = System.IO.File.ReadAllLines(
                        @"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\ReiningSires.txt");
                    horseType = "Reining";
                    break;

                case 2:
                    lines = System.IO.File.ReadAllLines(
                        @"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\CuttingSires.txt");
                    horseType = "Cutting";
                    break;

                case 3:
                    lines = System.IO.File.ReadAllLines(
                        @"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\CowHorses.txt");
                    horseType = "Cow Horse";
                    break;

                case 4:
                    lines = System.IO.File.ReadAllLines(
                        @"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\BarrelSires.txt");
                    horseType = "Barrel";
                    break;

                case 5:
                    lines = System.IO.File.ReadAllLines(
                        @"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\RopingSires.txt");
                    horseType = "Roping";
                    break;

                default:
                    lines = new string[0];
                    break;
            }

            foreach (string line in lines)
            {
                DataRow dr = dt.NewRow();
                dr[0] = line;
                dt.Rows.Add(dr);
            }


            //Initializing sql command, parameters, and connection

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    SqlCommand cmd = new SqlCommand("usp_v1_0_InsertSires");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    SqlParameter param2 = new SqlParameter();

                    param.ParameterName = "@Names";
                    param.Value = dt;
                    cmd.Parameters.Add(param);
                    param2.ParameterName = "@Type";
                    param2.Value = horseType;

                    cmd.Parameters.Add(param2);

                    System.Data.SqlClient.SqlConnection conn =
                        new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    //DataTable SireData;
                    /*
                    if (ds.Tables[0] != null)
                    {
                        SireData = ds.Tables[0];
                    }
                    else
                    {
                        SireData = new DataTable();
                    }
                    */
                    //close connection
                    context.Database.Connection.Close();
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}