using HorseApp2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace HorseApp2.Controllers
{
    //Main Controller
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

            if (headers.Contains("row"));
            {
                row = int.Parse(headers.GetValues("row").FirstOrDefault());
            }


            int exists = 0;
            string storedProcedureName = "usp_RowExists";
            string parameterSequence = "@row";

            SqlParameter[] sqlParameter =
            {
                    new SqlParameter { ParameterName = "@row", SqlDbType  = SqlDbType.BigInt, Value = row },

            };

            using (var context = new HorseDatabaseEntities())
            {
                exists = context.Database.SqlQuery<int>(storedProcedureName + " " + parameterSequence, sqlParameter).FirstOrDefault();
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
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_InsertActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = GetSqlParametersForInsert(listing);
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    //DataTable photos = ds.Tables[1];
                    DataTable photos = new DataTable();

                    var listings = DataTablesToHorseListing(listingData, photos);

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
            //////////////////////////////////////////////////////////
            //SqlParameter param8 = new SqlParameter();
            //param8.ParameterName = "@FirebaseId";
            //param8.Value = listing.FireBaseId;
            //////////////////////////////////////////////////////////
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
            dt.Columns.Add(ActiveListingPhotoIdColumn);
            dt.Columns.Add(ActiveListingIdColumn);
            dt.Columns.Add(PhotoURLColumn);
            dt.Columns.Add(PhotoOrderColumn);
            dt.Columns.Add(CreatedOnColumn);
            dt.Columns.Add(UpdatedOnColumn);

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
            //////////////////////////////////////////////////////////////////////////
            //parameters.Add(param8);
            /////////////////////////////////////////////////////////////////////////
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
            parameters.Add(photos);


            //response object
            HorseListing response = new HorseListing();
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_UpdateActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;
               
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    

                    var listings = DataTablesToHorseListing(listingData, photosFromDb);

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
        /// Creates parameters for the stored procedure: usp_InsertAcitveListing given a horseListing
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        private List<SqlParameter> GetSqlParametersForInsert(HorseListing listing)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter param0 = new SqlParameter();
            param0.ParameterName = "@ActiveListingId";
            param0.Value = listing.activeListingId;
            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@Age";
            /*
            if(listing.age == "weanling")
            {
                listing.age = "0";
            }
            else if(listing.age == "yearling")
            {
                listing.age = "1";
            }
            else if(listing.age[listing.age.Length - 1] == 'o')
            {
                listing.age = listing.age.Split(' ').ElementAt(0);
            }
            else if(listing.age[listing.age.Length - 1] == '+')
            {
                listing.age = "15";
            }
            */
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
            //////////////////////////////////////////////////////////
            //SqlParameter param8 = new SqlParameter();
            //param8.ParameterName = "@FirebaseId";
            //param8.Value = listing.FireBaseId;
            //////////////////////////////////////////////////////////
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

            /*
            SqlParameter photos = new SqlParameter();
            photos.ParameterName = "@Photos";
            photos.Value = ListingPhotoRequestToDataTable(listing.Photos);
            */

            parameters.Add(param0);
            parameters.Add(param1);
            parameters.Add(param2);
            parameters.Add(param3);
            parameters.Add(param5);
            parameters.Add(param6);
            parameters.Add(param7);
            //////////////////////////////////////////////////////////////////////////
            //parameters.Add(param8);
            /////////////////////////////////////////////////////////////////////////
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
            //parameters.Add(photos);

            return parameters;
        }

        /// <summary>
        /// Converts data from stored procedure into horse listing object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        private List<HorseListing> DataTablesToHorseListing(DataTable data, DataTable photos)
        {
            List<HorseListing> listings = new List<HorseListing>();
            HorseListing listing = new HorseListing();
            using (var context = new HorseDatabaseEntities())
            {
                foreach (DataRow row in data.Rows)
                {

                    List<DataRow> photosForRow = (from myRow in photos.AsEnumerable()
                                                  where myRow.Field<string>("ActiveListingId") == row["ActiveListingId"].ToString()
                                                  select myRow).ToList();



                    listings.Add(PopulateListing(row, photosForRow));

                }
            }

            return listings;
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
            List<long> ActiveListingIds = new List<long>();

            var request = this.Request;
            var headers = request.Headers;


            if (headers.Contains("ActiveListingIds")) ;
            {
                string ids = headers.GetValues("ActiveListingIds").First();

                //ids = ids.ToString();

                string[] idString = ids.ToString().Split(' ');

                for(int i = 0; i < idString.Length; i++)
                {
                    ActiveListingIds.Add(long.Parse(idString[i]));
                }
            }

            try
            {
                using(var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_DeleteActiveListing");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@ActiveListingIds";
                    param.Value = ListingIdListToDatatable(ActiveListingIds);
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    deleteHorseListings = DataTablesToHorseListing(listingData, photos);

                    //close connection
                    context.Database.Connection.Close();

                }

            }
            catch(Exception e)
            {
                e.ToString();
            }

            return deleteHorseListings;
        }

        /// <summary>
        /// Searches though active listings given filter fields
        /// TESTED AND WORKING
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchActiveListings")]
        public SearchResponse SearchActiveListings()
        {
            var request = this.Request;
            var headers = request.Headers;
            string name = "";
            SearchActiveListingsRequest objRequest = new SearchActiveListingsRequest();
            List<HorseListing> listings = new List<HorseListing>();
            SearchResponse response = new SearchResponse();
            string input = "";

                if (headers.Contains("types"))
                {
                    objRequest.TypeSearch = true;
                    input = headers.GetValues("types").First().Trim(new Char[] { '{', '}', '[',']' }).Replace(",","").Replace("\"", "");
                    objRequest.HorseTypes = input.Split(' ').ToList();
                }
                else
                {
                    objRequest.TypeSearch = false;
                    objRequest.HorseTypes = new List<string>();
                }

                if(headers.Contains("priceLow") || headers.Contains("priceHigh"))
                {
                    objRequest.PriceSearch = true;

                    if (headers.Contains("priceLow"))
                    {
                        objRequest.PriceLow = decimal.Parse(headers.GetValues("priceLow").First());
                    }
                    else
                    {
                    objRequest.PriceLow = 0;
                    }

                    if (headers.Contains("priceHigh"))
                    {
                        objRequest.PriceHigh = decimal.Parse(headers.GetValues("priceHigh").First());
                    }
                    else
                    {
                        objRequest.PriceHigh = 10000000;
                    }

                }
                else
                {
                    objRequest.PriceSearch = false;
                }

                if (headers.Contains("sires"))
                {
                    objRequest.SireSearch = true;
                    input = headers.GetValues("sires").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    objRequest.Sires = input.Split(' ').ToList();
                }
                else
                {
                    objRequest.SireSearch = false;
                    objRequest.Sires = new List<string>();
                }
            
           
           
                if (headers.Contains("genders"))
                {
                    objRequest.GenderSearch = true;
                    input = headers.GetValues("genders").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    objRequest.Genders = input.Split(' ').ToList();
                }
                else
                {
                    objRequest.GenderSearch = false;
                    objRequest.Genders = new List<string>();
                }
        
     
                if (headers.Contains("ages"))
                {
                    objRequest.AgeSearch = true;
                    input = headers.GetValues("ages").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    

                    string[] ages = input.Split(' ');
                    objRequest.Ages = new List<int>();
                    for (int i = 0; i < ages.Length; i++)
                    {
                        /*
                        if(ages[i] == "weanling")
                        {
                            ages[i] = "0";
                        }
                        else if(ages[i] == "yearling")
                        {
                            ages[i] = "1";
                        }
                        */
                        objRequest.Ages.Add(int.Parse(ages[i]));
                    }

                }
                else
                {
                    objRequest.AgeSearch = false;
                    objRequest.Ages = new List<int>();
                }
            
          
                if (headers.Contains("dams"))
                {
                    objRequest.DamSearch = true;
                    input = headers.GetValues("dams").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    objRequest.Dams = input.Split(' ').ToList();

                }
                else
                {
                    objRequest.DamSearch = false;
                    objRequest.Dams = new List<string>();
                }
           
         
                if (headers.Contains("damSires"))
                {
                    objRequest.DamSireSearch = true;
                    input = headers.GetValues("damSires").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    objRequest.DamSires = input.Split(' ').ToList();
                }
                else
                {
                    objRequest.DamSireSearch = false;
                    objRequest.DamSires = new List<string>();
                }
          
           
                if (headers.Contains("colors"))
                {
                    objRequest.ColorSearch = true;
                    input = headers.GetValues("colors").First().Trim(new Char[] { '{', '}', '[', ']' }).Replace(",", "").Replace("\"", "");
                    objRequest.Colors = input.Split(' ').ToList();
                }
                else
                {
                    objRequest.ColorSearch = false;
                    objRequest.Colors = new List<string>();
                }
        

          
                if(headers.Contains("lteHigh") || headers.Contains("lteLow"))
                {
                    objRequest.LteSearch = true;
                    if (headers.Contains("lteHigh"))
                    {
                        objRequest.LteHigh = decimal.Parse(headers.GetValues("lteHigh").First().ToString());
                    }
                    else
                    {
                        objRequest.LteHigh = 100000000;
                    }
                    if (headers.Contains("lteLow"))
                    {
                        objRequest.LteLow = decimal.Parse(headers.GetValues("lteLow").First().ToString());
                    }
                    else
                    {
                        objRequest.LteLow = 0;
                    }
                }

               
          
            
                if (headers.Contains("inFoal"))
                {
                    objRequest.InFoalSearch = true;
                    objRequest.InFoal = bool.Parse(headers.GetValues("inFoal").First());
                }
                else
                {
                    objRequest.InFoalSearch = false;
                    objRequest.InFoal = false;
                }
         
            if (headers.Contains("itemsPerPage"))
            {
                objRequest.ItemsPerPage = int.Parse(headers.GetValues("itemsPerPage").First());
            }
            else
            {
                objRequest.ItemsPerPage = 20;
            }
            if (headers.Contains("page"))
            {
                objRequest.Page = int.Parse(headers.GetValues("page").First());
            }
            else
            {
                objRequest.Page = 1;
            }
            if (headers.Contains("orderBy"))
            {
                objRequest.OrderBy = bool.Parse(headers.GetValues("orderBy").First());
            }
            else
            {
                objRequest.OrderBy = false;
            }
            if (headers.Contains("orderByType"))
            {
                objRequest.OrderByType = int.Parse(headers.GetValues("orderByType").First());
            }
            else
            {
                objRequest.OrderByType = 1;
            }
            if (headers.Contains("orderByDesc"))
            {
                objRequest.OrderByDesc = bool.Parse(headers.GetValues("orderByDesc").First());
            }
            else
            {
                objRequest.OrderByDesc = false;
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_SearchActiveListings");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = GetSqlParametersForSearchListings(objRequest);
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    DataTable photos = ds.Tables[1];
                    DataTable totalListings = ds.Tables[2];

                    int total = 0;
                    foreach(DataRow row in totalListings.Rows)
                    {
                        total = int.Parse(row["RowCount"].ToString());
                    }

                    //convert data from stored proceduure into ActiveListing object
                    response.listings = DataTablesToHorseListing(listingData, photos);
                    response.totalNumOfListings = total;
                    response.pageNumber = objRequest.Page;

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
        /// //Create and format Sql Parameters for stored procedure: usp_SearchActiveListings
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private List<SqlParameter> GetSqlParametersForSearchListings(SearchActiveListingsRequest request)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            DataTable dt4 = new DataTable();
            DataTable dt5 = new DataTable();
            DataTable dt6 = new DataTable();
            DataTable dt7 = new DataTable();

            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@TypeSearch";
            param1.SqlDbType = SqlDbType.Bit;
            param1.Value = request.TypeSearch;

            //////////////////////////////////////////////////////////////////////
            SqlParameter param2 = new SqlParameter();
            param2.SqlDbType = SqlDbType.Structured;
            param2.ParameterName = "@Types";
            //param2.Value = request.HorseTypes;

            DataColumn typeColumn = new DataColumn("HorseType");
            typeColumn.DataType = System.Type.GetType("System.String");

            dt1.Columns.Add(typeColumn);

            List<DataRow> rows = new List<DataRow>();
            int rowCount = request.HorseTypes.Count();
            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(dt1.NewRow());
            }
            int j = 0;
            foreach (DataRow row in rows)
            {
                row[0] = request.HorseTypes.ElementAt(j);
                dt1.Rows.Add(row);
                j++;
            }

            param2.Value = dt1;
            ////////////////////////////////////////////////////////////////////////////

            SqlParameter param3 = new SqlParameter();
            param3.ParameterName = "@PriceSearch";
            param3.Value = request.PriceSearch;
            SqlParameter param5 = new SqlParameter();
            param5.ParameterName = "@PriceLow";
            param5.Value = request.PriceLow;
            SqlParameter param6 = new SqlParameter();
            param6.ParameterName = "@PriceHigh";
            param6.Value = request.PriceHigh;
            SqlParameter param7 = new SqlParameter();
            param7.ParameterName = "@SireSearch";
            param7.Value = request.SireSearch;
            
            ///////////////////////////////////////////////////////////////////////////
            SqlParameter param8 = new SqlParameter();
            param8.ParameterName = "@Sires";
            //param8.Value = request.Sires;
            DataColumn nameColumn = new DataColumn("Name");
            nameColumn.DataType = System.Type.GetType("System.String");

            dt2.Columns.Add(nameColumn);

            List<DataRow> rows2 = new List<DataRow>();
            int rowCount2 = request.Sires.Count();
            for (int i = 0; i < rowCount2; i++)
            {
                rows2.Add(dt2.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows2)
            {
                row["Name"] = request.Sires.ElementAt(j);
                dt2.Rows.Add(row);
                j++;
            }

            param8.Value = dt2;
            ///////////////////////////////////////////////////////////////////////////

            SqlParameter param9 = new SqlParameter();
            param9.ParameterName = "@GenderSearch";
            param9.Value = request.GenderSearch;
            
            ///////////////////////////////////////////////////////////////////////////
            SqlParameter param10 = new SqlParameter();
            param10.ParameterName = "@Genders";
            //param10.Value = request.Genders;
            DataColumn genderColumn = new DataColumn("Gender");
            genderColumn.DataType = System.Type.GetType("System.String");

            dt3.Columns.Add(genderColumn);

            List<DataRow> rows3 = new List<DataRow>();
            int rowCount3 = request.Genders.Count();
            for (int i = 0; i < rowCount3; i++)
            {
                rows3.Add(dt3.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows3)
            {
                row["Gender"] = request.Genders.ElementAt(j);
                dt3.Rows.Add(row);
                j++;
            }

            param10.Value = dt3;
            ///////////////////////////////////////////////////////////////////////////

            SqlParameter param11 = new SqlParameter();
            param11.ParameterName = "@AgeSearch";
            param11.Value = request.AgeSearch;
            SqlParameter param12 = new SqlParameter();
            param12.ParameterName = "@Ages";

            //DataTable dt = new DataTable();
            DataColumn ageColumn = new DataColumn("Age");
            ageColumn.DataType = System.Type.GetType("System.Int32");

            dt4.Columns.Add(ageColumn);

            List<DataRow> rows4 = new List<DataRow>();
            int rowCount4 = request.Ages.Count();
            for (int i = 0; i < rowCount4; i++)
            {
                rows4.Add(dt4.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows4)
            {
                row["Age"] = request.Ages.ElementAt(j);
                dt4.Rows.Add(row);
                j++;
            }

            param12.Value = dt4;


            SqlParameter param19 = new SqlParameter();
            param19.ParameterName = "@DamSearch";
            param19.Value = request.DamSearch;

            //dams
            SqlParameter param13 = new SqlParameter();
            param13.ParameterName = "@Dams";

            //DataTable dt = new DataTable();
            DataColumn damsColumn = new DataColumn("Name");
            damsColumn.DataType = System.Type.GetType("System.String");

            dt5.Columns.Add(damsColumn);

            List<DataRow> rows5 = new List<DataRow>();
            int rowCount5 = request.Dams.Count();
            for (int i = 0; i < rowCount5; i++)
            {
                rows5.Add(dt5.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows5)
            {
                row["Name"] = request.Dams.ElementAt(j);
                dt5.Rows.Add(row);
                j++;
            }

            param13.Value = dt5;
            //


            SqlParameter param20 = new SqlParameter();
            param20.ParameterName = "@DamSireSearch";
            param20.Value = request.DamSireSearch;

            //damsires
            //
            SqlParameter param14 = new SqlParameter();
            param14.ParameterName = "@DamSires";

            //DataTable dt = new DataTable();
            DataColumn damSiresColumn = new DataColumn("Name");
            damSiresColumn.DataType = System.Type.GetType("System.String");

            dt6.Columns.Add(damSiresColumn);

            List<DataRow> rows6 = new List<DataRow>();
            int rowCount6 = request.DamSires.Count();
            for (int i = 0; i < rowCount6; i++)
            {
                rows6.Add(dt6.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows6)
            {
                row["Name"] = request.DamSires.ElementAt(j);
                dt6.Rows.Add(row);
                j++;
            }

            param14.Value = dt6;
            //

            SqlParameter param21 = new SqlParameter();
            param21.ParameterName = "@ColorSearch";
            param21.Value = request.ColorSearch;

            //colors
            //
            SqlParameter param15 = new SqlParameter();
            param15.ParameterName = "@Colors";

            //DataTable dt = new DataTable();
            DataColumn colorsColumn = new DataColumn("Color");
            colorsColumn.DataType = System.Type.GetType("System.String");

            dt7.Columns.Add(colorsColumn);

            List<DataRow> rows7 = new List<DataRow>();
            int rowCount7 = request.Colors.Count();
            for (int i = 0; i < rowCount7; i++)
            {
                rows7.Add(dt7.NewRow());
            }
            j = 0;
            foreach (DataRow row in rows7)
            {
                row["Color"] = request.Colors.ElementAt(j);
                dt7.Rows.Add(row);
                j++;
            }

            param15.Value = dt7;
            //

            SqlParameter param22 = new SqlParameter();
            param22.ParameterName = "@LteSearch";
            param22.Value = request.LteSearch;

            //lteHigh
            //
            SqlParameter param16 = new SqlParameter();
            param16.ParameterName = "@LteHigh";
            param16.Value = request.LteHigh;
            //

            //lteLow
            //
            SqlParameter param17 = new SqlParameter();
            param17.ParameterName = "@LteLow";
            param17.Value = request.LteLow;
            //

            SqlParameter param23 = new SqlParameter();
            param23.ParameterName = "@InFoalSearch";
            param23.Value = request.InFoalSearch;

            //InFoal
            //
            SqlParameter param18 = new SqlParameter();
            param18.ParameterName = "@InFoal";
            param18.Value = request.InFoal;
            //


            SqlParameter param24 = new SqlParameter();
            param24.ParameterName = "@ItemsPerPage";
            param24.Value = request.ItemsPerPage;
            SqlParameter param25 = new SqlParameter();
            param25.ParameterName = "@Page";
            param25.Value = request.Page;
            SqlParameter param26 = new SqlParameter();
            param26.ParameterName = "@OrderBy";
            param26.Value = request.OrderBy;
            SqlParameter param27 = new SqlParameter();
            param27.ParameterName = "@OrderByType";
            param27.Value = request.OrderByType;
            SqlParameter param28 = new SqlParameter();
            param28.ParameterName = "@OrderByDesc";
            param28.Value = request.OrderByDesc;


            parameters.Add(param1);
            parameters.Add(param2);
            parameters.Add(param3);
            parameters.Add(param5);
            parameters.Add(param6);
            parameters.Add(param7);
            parameters.Add(param8);
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
            parameters.Add(param21);
            parameters.Add(param22);
            parameters.Add(param23);
            parameters.Add(param24);
            parameters.Add(param25);
            parameters.Add(param26);
            parameters.Add(param27);
            parameters.Add(param28);

            return parameters;
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
            if(headers.Contains("HorseType"))
            {
                horseType = headers.GetValues("horseType").First().ToString();
            }

            try
            {
                using(var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_InsertSire");
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@HorseType";
                    param2.Value = horseType;
                    cmd.Parameters.Add(param2);

                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
                    cmd.Connection = conn;

                    //open connection
                    context.Database.Connection.Open();

                    //execute and retrieve data from stored procedure
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    DataTable SireData;
                    if(ds.Tables[0] != null)
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
            catch(Exception e)
            {
                e.ToString();
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

                using(var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_DeleteSire");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
            catch(Exception e)
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
                    SqlCommand cmd = new SqlCommand("usp_SearchAllSires");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@Name";
                    param.Value = name;
                    cmd.Parameters.Add(param);
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    foreach(SireResponse s in listFromDB)
                    {
                        response[j].Add(s);
                        i++;
                        if(i == 19)
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
            catch(Exception e)
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
            if(headers.Contains("horseTypes"))
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
                    SqlCommand cmd = new SqlCommand("usp_SearchAllSiresElastically");
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
                    
                    for(int i = 0; i < horseTypes.Length; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = horseTypes[i];
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




                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    SqlCommand cmd = new SqlCommand("usp_searchByPrice");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();
                    SqlParameter param2 = new SqlParameter();

                    
                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    response = DataTablesToHorseListing(listingData, photos);

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
                    SqlCommand cmd = new SqlCommand("usp_SearchByHorseType");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    response = DataTablesToHorseListing(listingData, photos);

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
                    SqlCommand cmd = new SqlCommand("usp_SearchBySire");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    response = DataTablesToHorseListing(listingData, photos);

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
                    SqlCommand cmd = new SqlCommand("usp_SearchByGender");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    response = DataTablesToHorseListing(listingData, photos);

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

            List<HorseListing> response = new List<HorseListing>();
            List<int> ages = new List<int>();


            if (headers.Contains("Ages"))
            {
                string[] agesArray = headers.GetValues("Ages").First().Split(' ');

                for(int i = 0; i < agesArray.Length; i++)
                {
                    ages.Add(int.Parse(agesArray[i]));
                }
            }



            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_SearchByAge");
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter param1 = new SqlParameter();


                    System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
                    response = DataTablesToHorseListing(listingData, photos);

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
            foreach(DataRow row in count.Rows)
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

            foreach(DataRow row in sireTable.Rows)
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

        /// <summary>
        /// Populates an Active Listing object with data rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        private HorseListing PopulateListing(DataRow row, List<DataRow> photos)
        {
            HorseListing listing = new HorseListing();

            listing.activeListingId = row["ActiveListingId"].ToString();
            /*
            if(row["Age"].ToString() == "0")
            {
                listing.age = "weanling";
            }
            else if(row["Age"].ToString() == "1")
            {
                listing.age = "yearling";
            }
            else
            {
                listing.age = row["Age"].ToString();
            }
            */
            listing.age = int.Parse(row["Age"].ToString());
            listing.color = row["Color"].ToString();
            listing.dam = row["Dam"].ToString();
            listing.sire = row["Sire"].ToString();
            listing.damSire = row["DamSire"].ToString();
            listing.description = row["Description"].ToString();
            //listing.fireBaseId = row["FirebaseId"].ToString();
            listing.gender = row["Gender"].ToString();
            listing.horseName = row["HorseName"].ToString();
            listing.inFoal = bool.Parse(row["InFoal"].ToString()); 
            listing.lte = decimal.Parse(row["Lte"].ToString());
            listing.originalDateListed = row["OriginalDateListed"].ToString();
            listing.price = decimal.Parse(row["Price"].ToString());
            listing.purchaseListingType = row["PurchaseListingType"].ToString();
            listing.ranchPhoto = row["RanchPhoto"].ToString();
            listing.sellerId = row["SellerId"].ToString();
            listing.horseType = row["HorseType"].ToString();
            listing.isSold = bool.Parse(row["IsSold"].ToString()); 

            int i = 0;
            foreach(DataRow dr in photos)
            {
                listing.photos.Add(new HorseListingPhoto());
                listing.photos.ElementAt(i).activeListingPhotoId = long.Parse(dr["ActiveListingPhotoId"].ToString());
                listing.photos.ElementAt(i).activeListingId = dr["ActiveListingId"].ToString();
                listing.photos.ElementAt(i).photoUrl = dr["PhotoURL"].ToString();
                listing.photos.ElementAt(i).photoOrder = int.Parse(dr["PhotoOrder"].ToString());
                listing.photos.ElementAt(i).createdOn = dr["CreatedOn"].ToString();
                listing.photos.ElementAt(i).updatedOn = dr["UpdatedOn"].ToString();

                i++;
            }

            return listing;
        }

        /// <summary>
        /// Converts data from a request object for inserting an ActiveListing into a datatable
        /// Creates readable data for sp
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        private DataTable ListingDataRequestToDataTable(HorseListing listing)
        {
            DataTable dt = new DataTable();
            DataColumn AgeColumn = new DataColumn("Age");
            AgeColumn.DataType = System.Type.GetType("System.String");
            DataColumn ColorColumn = new DataColumn("Color");
            ColorColumn.DataType = System.Type.GetType("System.String");
            DataColumn DamColumn = new DataColumn("Dam");
            DamColumn.DataType = System.Type.GetType("System.String");
            DataColumn SireColumn = new DataColumn("Sire");
            SireColumn.DataType = System.Type.GetType("System.String");
            DataColumn DamSireColumn = new DataColumn("DamSire");
            DamSireColumn.DataType = System.Type.GetType("System.String"); 
            DataColumn DescriptionColumn = new DataColumn("Description");
            DescriptionColumn.DataType = System.Type.GetType("System.String");
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DataColumn FirebaseIdColumn = new DataColumn("FirebaseId");
            FirebaseIdColumn.DataType = System.Type.GetType("System.String"); 
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DataColumn GenderColumn = new DataColumn("Gender");
            GenderColumn.DataType = System.Type.GetType("System.String"); 
            DataColumn HorseNameColumn = new DataColumn("HorseName");
            HorseNameColumn.DataType = System.Type.GetType("System.String"); 
            DataColumn InForalColumn = new DataColumn("InFoal");
            InForalColumn.DataType = System.Type.GetType("System.Boolean");
            DataColumn LteColumn = new DataColumn("Lte");
            LteColumn.DataType = System.Type.GetType("System.Decimal");
            DataColumn OriginalDateListedColumn = new DataColumn("OriginalDateListed");
            OriginalDateListedColumn.DataType = System.Type.GetType("System.DateTime"); 
            DataColumn PriceColumn = new DataColumn("Price");
            PriceColumn.DataType = System.Type.GetType("System.Decimal");
            DataColumn PurchaseListingTypeColumn = new DataColumn("PurchaseListingType");
            PurchaseListingTypeColumn.DataType = System.Type.GetType("System.String");
            DataColumn RanchPhotoColumn = new DataColumn("RanchPhoto");
            RanchPhotoColumn.DataType = System.Type.GetType("System.String");
            DataColumn SellerIdColumn = new DataColumn("SellerId");
            SellerIdColumn.DataType = System.Type.GetType("System.String");
            DataColumn HorseTypeColumn = new DataColumn("HorseType");
            HorseTypeColumn.DataType = System.Type.GetType("System.String");
            DataColumn IsSoldColumn = new DataColumn("IsSold");
            IsSoldColumn.DataType = System.Type.GetType("System.Boolean");

            dt.Columns.Add(AgeColumn);
            dt.Columns.Add(ColorColumn);
            dt.Columns.Add(DamColumn);
            dt.Columns.Add(SireColumn);
            dt.Columns.Add(DamSireColumn);
            dt.Columns.Add(DescriptionColumn);
            dt.Columns.Add(FirebaseIdColumn);
            dt.Columns.Add(GenderColumn);
            dt.Columns.Add(HorseNameColumn);
            dt.Columns.Add(InForalColumn);
            dt.Columns.Add(LteColumn);
            dt.Columns.Add(OriginalDateListedColumn);
            dt.Columns.Add(PriceColumn);
            dt.Columns.Add(PurchaseListingTypeColumn);
            dt.Columns.Add(RanchPhotoColumn);
            dt.Columns.Add(SellerIdColumn);
            dt.Columns.Add(HorseTypeColumn);
            dt.Columns.Add(IsSoldColumn);

            var newRow = dt.NewRow();
            newRow["Age"] = listing.age;
            newRow["Color"] = listing.color;
            newRow["Dam"] = listing.dam;
            newRow["Sire"] = listing.sire;
            newRow["DamSire"] = listing.damSire;
            newRow["Description"] = listing.description;
            //newRow["FirebaseId"] = listing.fireBaseId;
            newRow["Gender"] = listing.gender;
            newRow["HorseName"] = listing.horseName;
            newRow["InFoal"] = listing.inFoal;
            newRow["Lte"] = listing.lte;
            newRow["OriginalDateListed"] = listing.originalDateListed;
            newRow["Price"] = listing.price;
            newRow["PurchaseListingType"] = listing.purchaseListingType;
            newRow["RanchPhoto"] = listing.ranchPhoto;
            newRow["SellerId"] = listing.sellerId;
            newRow["HorseType"] = listing.horseType;
            newRow["IsSold"] = listing.isSold;

            dt.Rows.Add(newRow);

            return dt;
        }

        /// <summary>
        /// Converts photos from request to insert an Active Listing into a datatable
        /// Creates readable data for sp
        /// </summary>
        /// <param name="photos"></param>
        /// <returns></returns>
        private DataTable ListingPhotoRequestToDataTable(List<HorseListingPhoto> photos)
        {
            DataTable dt = new DataTable();
            DataColumn PhotoURLColumn = new DataColumn("PhotoURL");
            PhotoURLColumn.DataType = System.Type.GetType("System.String");
            DataColumn PhotoOrderColumn = new DataColumn("PhotoOrder");
            PhotoOrderColumn.DataType = System.Type.GetType("System.Int32");
            DataColumn CreatedOnColumn = new DataColumn("CreatedOn");
            CreatedOnColumn.DataType = System.Type.GetType("System.DateTime");
            DataColumn UpdatedOnColumn = new DataColumn("UpdatedOn");
            UpdatedOnColumn.DataType = System.Type.GetType("System.DateTime");
            dt.Columns.Add(PhotoURLColumn);
            dt.Columns.Add(PhotoOrderColumn);
            dt.Columns.Add(CreatedOnColumn);
            dt.Columns.Add(UpdatedOnColumn);

            List<DataRow> rows = new List<DataRow>();
            int rowCount = photos.Count();
            for(int i = 0; i < rowCount; i++)
            {
                rows.Add(dt.NewRow());
            }
            int j = 0;
            HorseListingPhoto photo;
            foreach(DataRow row in rows)
            {
                photo = photos.ElementAt(j);

                row["PhotoURL"] = photo.photoUrl;
                row["PhotoOrder"] = photo.photoOrder;
                row["CreatedOn"] = photo.createdOn;
                row["UpdatedOn"] = photo.updatedOn;

                dt.Rows.Add(row);

                j++;
            }

            return dt;
        }

        

        private DataTable ListingIdListToDatatable(List<long> Ids)
        {

            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("ActiveListingId");
            column.DataType = System.Type.GetType("System.Int64");
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

            if(headers.Contains("type"))
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
                    lines = System.IO.File.ReadAllLines(@"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\ReiningSires.txt");
                    horseType = "Reining";
                    break;

                case 2:
                    lines = System.IO.File.ReadAllLines(@"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\CuttingSires.txt");
                    horseType = "Cutting";
                    break;

                case 3:
                    lines = System.IO.File.ReadAllLines(@"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\CowHorses.txt");
                    horseType = "Cow Horse";
                    break;

                case 4:
                    lines = System.IO.File.ReadAllLines(@"C:\Users\Aric\Source\repos\HorseApp2\HorseApp2\BarrelSires.txt");
                    horseType = "Barrel";
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
                        SqlCommand cmd = new SqlCommand("usp_InsertSires");
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter param = new SqlParameter();
                        SqlParameter param2 = new SqlParameter();

                        param.ParameterName = "@Names";
                        param.Value = dt;
                        cmd.Parameters.Add(param);
                        param2.ParameterName = "@Type";
                        param2.Value = horseType;
                        
                        cmd.Parameters.Add(param2);

                        System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(context.Database.Connection.ConnectionString);
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
