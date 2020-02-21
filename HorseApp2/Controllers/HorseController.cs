using HorseApp2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HorseApp2.Controllers
{
    //Main Controller
    public class HorseController : ApiController
    {

        //WORKS WITH ROW IN HEADERS
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
                    DataTable photos = ds.Tables[1];

                    //convert data from stored proceduure into ActiveListing object
                    response = DataTablesToHorseListing(listingData, photos).ElementAt(0);

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

      

        [HttpPost]
        [Route("InsertSire")]
        public SireResponse InsertSire(string name)
        {
            SireResponse response = new SireResponse();

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
                    response = SireTableToSireResponse(SireData).ElementAt(0);

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


        [HttpDelete]
        [Route("DeleteSire")]
        public SireResponse DeleteSire()
        {
            SireResponse response = new SireResponse();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";

            if (headers.Contains("Name")) ;
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
                    response = SireTableToSireResponse(SireData).ElementAt(0);

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

        [HttpGet]
        [Route("SearchAllSires")]
        public List<SireResponse> SearchAllSires()
        {
            List<SireResponse> response = new List<SireResponse>();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";

            if (headers.Contains("Name")) ;
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


                    //convert data from stored procedure into Sire object
                    response = SireTableToSireResponse(SireData);

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

        [HttpGet]
        [Route("SearchAllSiresElastically")]
        public List<SireResponse> SearchAllSiresElastically()
        {
            List<SireResponse> response = new List<SireResponse>();
            var request = this.Request;
            var headers = request.Headers;
            string name = "";

            if (headers.Contains("Name")) ;
            {
                name = headers.GetValues("Name").First();
            }

            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    //Initializing sql command, parameters, and connection
                    SqlCommand cmd = new SqlCommand("usp_SearchAllSiresElastically");
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
                    response = SireTableToSireResponse(SireData);

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


        [HttpGet]
        [Route("SearchActiveListings")]
        public void SearchActiveListings()
        {
            var request = this.Request;
            var headers = request.Headers;
            string name = "";
            SearchActiveListingsRequest objRequest = new SearchActiveListingsRequest();

            if (headers.Contains("TypeSearch")) 
            {
                objRequest.TypeSearch = bool.Parse(headers.GetValues("TypeSearch").ToString());

                if (headers.Contains("Type"))
                {
                    objRequest.HorseType = headers.GetValues("Type").ToString();
                }
            }
            if (headers.Contains("PriceSearch"))
            {
                objRequest.PriceSearch = bool.Parse(headers.GetValues("PriceSearch").ToString());

                if (headers.Contains("PriceLow"))
                {
                    objRequest.PriceLow = decimal.Parse(headers.GetValues("PriceLow").ToString());
                }
                else
                {
                    objRequest.PriceLow = 0;
                }
                if(headers.Contains("PriceHigh"))
                {
                    objRequest.PriceHigh = decimal.Parse(headers.GetValues("PriceHigh").ToString());
                }
                else
                {
                    objRequest.PriceHigh = 10000000;
                }
            }
            if (headers.Contains("SireSearch"))
            {
                objRequest.SireSearch = bool.Parse(headers.GetValues("SireSearch").ToString());

                if (headers.Contains("Sire"))
                {
                    objRequest.Sire = headers.GetValues("Sire").ToString();
                }
            }
            if (headers.Contains("GenderSearch"))
            {
                objRequest.GenderSearch = bool.Parse(headers.GetValues("GenderSearch").ToString());

                if (headers.Contains("Gender"))
                {
                    objRequest.Gender = headers.GetValues("Gender").ToString();
                }
            }
            if (headers.Contains("AgeSearch"))
            {
                objRequest.AgeSearch = bool.Parse(headers.GetValues("AgeSearch").ToString());

                if (headers.Contains("Ages"))
                {
                   string[] ages = headers.GetValues("Ages").ToString().Split(' ');
                    for(int i = 0; i < ages.Length; i++)
                    {
                        objRequest.Ages.Add(int.Parse(ages[i]));
                    }
                    
                }
            }

            try
            {
                using(var context = new HorseDatabaseEntities())
                {

                }
            }
            catch(Exception e)
            {
                e.ToString();
            }

        }


          


        /// <summary>
        /// Retrieves all Active Listings within a specified price range
        /// NOT COMPLETE
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SearchByPrice")]
        public SearchByPriceResponse SearchByPrice()
        {
            //generic values for price range if none are found
            decimal Low = 0;
            decimal High = 1000000;
            
            var request = this.Request;
            var headers = request.Headers;

            HorseListingData listingData;
            List<HorseListingData> allListingData;
            HorseListingPhoto listingPhoto;
            List<HorseListing> listings;
            SearchByPriceResponse response = new SearchByPriceResponse();

            //populate price range
            if (headers.Contains("Low"))
            {
                Low = decimal.Parse(headers.GetValues("Low").FirstOrDefault());
            }
            if(headers.Contains("High"))
            {
                High = decimal.Parse(headers.GetValues("High").FirstOrDefault());
            }


            using(var context = new HorseDatabaseEntities())
            {
                //creating command for stored procedure
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "[dbo].[usp_searchByPrice]" + " " + Low + ", " + High;

                try
                {
                    //open connection
                    context.Database.Connection.Open();
                    
                    //execute and retrieve data from stored procedure
                    var reader = cmd.ExecuteReader();
                    allListingData = ((IObjectContextAdapter)context).ObjectContext.Translate<HorseListingData>(reader, "tblActiveListings", MergeOption.AppendOnly).ToList();

                    foreach(var data in allListingData)
                    {
                        Console.WriteLine(data.ActiveListingId);

                    }
                }
                catch(Exception e)
                {
                    e.ToString();
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

            return response;
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
            using(var context = new HorseDatabaseEntities())
            {
                foreach (DataRow row in data.Rows)
                {
                    
                    List<DataRow> photosForRow = (from myRow in photos.AsEnumerable()
                                                 where myRow.Field<long>("ActiveListingId") == long.Parse(row["ActiveListingId"].ToString())
                                                 select myRow).ToList();
                   
                       
                     
                    listings.Add(PopulateListing(row, photosForRow));

                }
            }
          
            return listings;
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

            response.SireServerId = long.Parse(row["SireServerId"].ToString());
            response.Name = row["Name"].ToString();
            response.CreatedOn = row["CreatedOn"].ToString();

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

            listing.ActiveListingId = long.Parse(row["ActiveListingId"].ToString());
            listing.Age = int.Parse(row["Age"].ToString());
            listing.Color = row["Color"].ToString();
            listing.Dam = row["Dam"].ToString();
            listing.Sire = row["Sire"].ToString();
            listing.DamSire = row["DamSire"].ToString();
            listing.Description = row["Description"].ToString();
            listing.FireBaseId = row["FirebaseId"].ToString();
            listing.Gender = row["Gender"].ToString();
            listing.HorseName = row["HorseName"].ToString();
            listing.InFoal = bool.Parse(row["InFoal"].ToString());
            listing.Lte = decimal.Parse(row["Lte"].ToString());
            listing.OriginalDateListed = row["OriginalDateListed"].ToString();
            listing.Price = decimal.Parse(row["Price"].ToString());
            listing.PurchaseListingType = row["PurchaseListingType"].ToString();
            listing.RanchPhoto = row["RanchPhoto"].ToString();
            listing.SellerId = row["SellerId"].ToString();
            listing.HorseType = row["HorseType"].ToString();

            int i = 0;
            foreach(DataRow dr in photos)
            {
                listing.Photos.Add(new HorseListingPhoto());
                listing.Photos.ElementAt(i).ActiveListingPhotoId = long.Parse(dr["ActiveListingPhotoId"].ToString());
                listing.Photos.ElementAt(i).ActiveListingId = long.Parse(dr["ActiveListingId"].ToString());
                listing.Photos.ElementAt(i).PhotoURL = dr["PhotoURL"].ToString();
                listing.Photos.ElementAt(i).PhotoOrder = int.Parse(dr["PhotoOrder"].ToString());
                listing.Photos.ElementAt(i).CreatedOn = dr["CreatedOn"].ToString();
                listing.Photos.ElementAt(i).UpdatedOn = dr["UpdatedOn"].ToString();

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
            AgeColumn.DataType = System.Type.GetType("System.Int32");
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
            DataColumn FirebaseIdColumn = new DataColumn("FirebaseId");
            FirebaseIdColumn.DataType = System.Type.GetType("System.String"); 
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

            var newRow = dt.NewRow();
            newRow["Age"] = listing.Age;
            newRow["Color"] = listing.Color;
            newRow["Dam"] = listing.Dam;
            newRow["Sire"] = listing.Sire;
            newRow["DamSire"] = listing.DamSire;
            newRow["Description"] = listing.Description;
            newRow["FirebaseId"] = listing.FireBaseId;
            newRow["Gender"] = listing.Gender;
            newRow["HorseName"] = listing.HorseName;
            newRow["InFoal"] = listing.InFoal;
            newRow["Lte"] = listing.Lte;
            newRow["OriginalDateListed"] = listing.OriginalDateListed;
            newRow["Price"] = listing.Price;
            newRow["PurchaseListingType"] = listing.PurchaseListingType;
            newRow["RanchPhoto"] = listing.RanchPhoto;
            newRow["SellerId"] = listing.SellerId;
            newRow["HorseType"] = listing.HorseType;

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

                row["PhotoURL"] = photo.PhotoURL;
                row["PhotoOrder"] = photo.PhotoOrder;
                row["CreatedOn"] = photo.CreatedOn;
                row["UpdatedOn"] = photo.UpdatedOn;

                dt.Rows.Add(row);

                j++;
            }

            return dt;
        }

        /// <summary>
        /// Creates parameters for sp given a horseListing
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        private List<SqlParameter> GetSqlParametersForInsert(HorseListing listing)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@Age";
            param1.Value = listing.Age;
            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@Color";
            param2.Value = listing.Color;
            SqlParameter param3 = new SqlParameter();
            param3.ParameterName = "@Dam";
            param3.Value = listing.Dam;
            SqlParameter param5 = new SqlParameter();
            param5.ParameterName = "@Sire";
            param5.Value = listing.Sire;
            SqlParameter param6 = new SqlParameter();
            param6.ParameterName = "@DamSire";
            param6.Value = listing.DamSire;
            SqlParameter param7 = new SqlParameter();
            param7.ParameterName = "@Description";
            param7.Value = listing.Description;
            SqlParameter param8 = new SqlParameter();
            param8.ParameterName = "@FirebaseId";
            param8.Value = listing.FireBaseId;
            SqlParameter param9 = new SqlParameter();
            param9.ParameterName = "@Gender";
            param9.Value = listing.Gender;
            SqlParameter param10 = new SqlParameter();
            param10.ParameterName = "@HorseName";
            param10.Value = listing.HorseName;
            SqlParameter param11 = new SqlParameter();
            param11.ParameterName = "@InFoal";
            param11.Value = listing.InFoal;
            SqlParameter param12 = new SqlParameter();
            param12.ParameterName = "@Lte";
            param12.Value = listing.Lte;
            SqlParameter param13 = new SqlParameter();
            param13.ParameterName = "@OriginalDateListed";
            param13.Value = listing.OriginalDateListed;
            SqlParameter param14 = new SqlParameter();
            param14.ParameterName = "@Price";
            param14.Value = listing.Price;
            SqlParameter param15 = new SqlParameter();
            param15.ParameterName = "@PurchaseListingType";
            param15.Value = listing.PurchaseListingType;
            SqlParameter param16 = new SqlParameter();
            param16.ParameterName = "@RanchPhoto";
            param16.Value = listing.RanchPhoto;
            SqlParameter param17 = new SqlParameter();
            param17.ParameterName = "@SellerId";
            param17.Value = listing.SellerId;
            SqlParameter param18 = new SqlParameter();
            param18.ParameterName = "@HorseType";
            param18.Value = listing.HorseType;

            
            SqlParameter photos = new SqlParameter();
            photos.ParameterName = "@Photos";
            photos.Value = ListingPhotoRequestToDataTable(listing.Photos);
            
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
            parameters.Add(photos);

            return parameters;
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

      

       
        
    }
}
