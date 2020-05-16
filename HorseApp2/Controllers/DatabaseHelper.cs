using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HorseApp2.Models;

namespace HorseApp2.Controllers
{
    public class DatabaseHelper
    {
        /// <summary>
        /// Converts data from stored procedure into horse listing object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        public List<HorseListing> DataTablesToHorseListing(DataTable data, DataTable photos)
        {
            try
            {
                using (var context = new HorseDatabaseEntities())
                {
                    var listings = new List<HorseListing>();
                    foreach (DataRow row in data.Rows)
                    {
                        List<DataRow> photosForRow = (from myRow in photos.AsEnumerable()
                            where myRow.Field<string>("ActiveListingId") == row["ActiveListingId"].ToString()
                            select myRow).ToList();

                        listings.Add(PopulateListing(row, photosForRow));
                    }

                    return listings;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return new List<HorseListing>();
            }
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
            listing.age = int.Parse(row["Age"].ToString());
            listing.color = row["Color"].ToString();
            listing.dam = row["Dam"].ToString();
            listing.sire = row["Sire"].ToString();
            listing.damSire = row["DamSire"].ToString();
            listing.description = row["Description"].ToString();
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
            listing.InFoalTo = row["InFoalTo"].ToString();
            listing.callForPrice = bool.Parse(row["CallForPrice"].ToString());
            listing.Height = double.Parse(row["Height"].ToString());
            listing.IsSireRegistered = bool.Parse(row["IsSireRegistered"].ToString());
            listing.IsDamSireRegistered = bool.Parse(row["IsDamSireRegistered"].ToString());

            int i = 0;
            foreach (DataRow dr in photos)
            {
                listing.photos.Add(new HorseListingPhoto());
                listing.photos.ElementAt(i).activeListingPhotoId = long.Parse(dr["ActiveListingPhotoId"].ToString());
                listing.photos.ElementAt(i).activeListingId = dr["ActiveListingId"].ToString();
                listing.photos.ElementAt(i).photoUrl = dr["PhotoURL"].ToString();
                listing.photos.ElementAt(i).photoOrder = int.Parse(dr["PhotoOrder"].ToString());
                listing.photos.ElementAt(i).createdOn = dr["CreatedOn"].ToString();
                listing.photos.ElementAt(i).updatedOn = dr["UpdatedOn"].ToString();
                listing.photos.ElementAt(i).isVideo = bool.Parse(dr["IsVideo"].ToString());

                i++;
            }

            return listing;
        }
        
        /// <summary>
        /// Create and format Sql Parameters for stored procedure: usp_SearchActiveListings
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<SqlParameter> GetSqlParametersForSearchListings(SearchActiveListingsRequest request)
        {

            var parameters = new List<SqlParameter>();
            
            parameters.Add(BuildSqlParameter("@TypeSearch", request.TypeSearch, SqlDbType.Bit));
            parameters.Add(BuildSqlParameter("@Types", BuildHorseTypeValue(request.HorseTypes), SqlDbType.Structured));
            parameters.Add(BuildSqlParameter("@PriceSearch", request.PriceSearch));
            parameters.Add(BuildSqlParameter("@PriceLow", request.PriceLow));
            parameters.Add(BuildSqlParameter("@PriceHigh", request.PriceHigh));
            parameters.Add(BuildSqlParameter("@SireSearch", request.SireSearch));
            parameters.Add(BuildSqlParameter("@Sires", BuildSqlParameterValue(request.Sires, "Name", "System.String")));
            parameters.Add(BuildSqlParameter("@GenderSearch", request.GenderSearch));
            parameters.Add(BuildSqlParameter("@Genders", BuildSqlParameterValue(request.Genders, "Gender", "System.String")));
            parameters.Add(BuildSqlParameter("@AgeSearch", request.AgeSearch));
            parameters.Add(BuildSqlParameter("@Ages", BuildSqlParameterValue(request.Ages, "Age", "System.Int32")));
            parameters.Add(BuildSqlParameter("@DamSearch", request.DamSearch));
            parameters.Add(BuildSqlParameter("@Dams", BuildSqlParameterValue(request.Dams, "Name", "System.String")));
            parameters.Add(BuildSqlParameter("@DamSireSearch", request.DamSireSearch));
            parameters.Add(BuildSqlParameter("@DamSires", BuildSqlParameterValue(request.DamSires, "Name", "System.String")));
            parameters.Add(BuildSqlParameter("@ColorSearch", request.ColorSearch));
            parameters.Add(BuildSqlParameter("@Colors", BuildSqlParameterValue(request.Colors, "Color", "System.String")));
            parameters.Add(BuildSqlParameter("@LteSearch", request.LteSearch));
            parameters.Add(BuildSqlParameter("@LteHigh", request.LteHigh));
            parameters.Add(BuildSqlParameter("@LteLow", request.LteLow));
            parameters.Add(BuildSqlParameter("@InFoalSearch", request.InFoalSearch));
            parameters.Add(BuildSqlParameter("@InFoal", request.InFoal));
            parameters.Add(BuildSqlParameter("@ItemsPerPage", request.ItemsPerPage));
            parameters.Add(BuildSqlParameter("@Page", request.Page));
            parameters.Add(BuildSqlParameter("@OrderBy", request.OrderBy));
            parameters.Add(BuildSqlParameter("@OrderByType", request.OrderByType));
            parameters.Add(BuildSqlParameter("@OrderByDesc", request.OrderByDesc));
            parameters.Add(BuildSqlParameter("@ActiveListingIdSearch", request.ActiveListingIdSearch));
            parameters.Add(BuildSqlParameter("ActiveListingIds", BuildSqlParameterValue(request.ActiveListingIds, "IDs", "System.String")));
            parameters.Add(BuildSqlParameter("@InFoalToSearch", request.InFoalSearch));
            parameters.Add(BuildSqlParameter("@InFoalTo", BuildSqlParameterValue(request.InFoalTo, "InFoalTo", "System.String")));
            parameters.Add(BuildSqlParameter("@IsSoldSearch", request.isSoldSearch));
            parameters.Add(BuildSqlParameter("@IsSold", request.IsSold));
            parameters.Add(BuildSqlParameter("@HeightSearch", request.HeightSearch));
            parameters.Add(BuildSqlParameter("@Heights", BuildSqlParameterValue(request.Heights, "Height", "System.Double")));
            parameters.Add(BuildSqlParameter("IsSireRegisteredSearch", request.IsSireRegisteredSearch));
            parameters.Add(BuildSqlParameter("IsSireRegistered", request.IsSireRegistered));
            parameters.Add(BuildSqlParameter("IsDamSireRegisteredSearch", request.IsDamSireRegisteredSearch));
            parameters.Add(BuildSqlParameter("IsDamSireRegistered", request.IsDamSireRegistered));
            // parameters.Add(BuildSqlParameter("@LocationsSearch", request.LocationsSearch));
            // parameters.Add(BuildSqlParameter("@Locations", BuildSqlParameterValue(request.Locations, "string", "System.String")));

            return parameters;
        }

        private SqlParameter BuildSqlParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        private SqlParameter BuildSqlParameter(string name, object value, SqlDbType type)
        {
            var param = BuildSqlParameter(name, value);
            param.SqlDbType = type;
            return param;
        }

        private DataTable BuildHorseTypeValue(List<string> horseTypes)
        {
            var dataTable = new DataTable();
            DataColumn typeColumn = new DataColumn("HorseType");
            typeColumn.DataType = Type.GetType("System.String");

            dataTable.Columns.Add(typeColumn);

            List<DataRow> rows = new List<DataRow>();
            if (horseTypes == null)
            {
                return dataTable;
            }
            int rowCount = horseTypes.Count();
            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(dataTable.NewRow());
            }
            int j = 0;
            foreach (DataRow row in rows)
            {
                if(horseTypes.ElementAt(j) == "cowHorse")
                {
                    row[0] = "cow horse";
                }
                else
                {
                    row[0] = horseTypes.ElementAt(j);
                }
                
                dataTable.Rows.Add(row);
                j++;
            }

            return dataTable;
        }

        private DataTable BuildSqlParameterValue<T>(List<T> list, string columnName, string typeName)
        {
            var column = new DataColumn(columnName);
            column.DataType = Type.GetType(typeName);
            var dataTable = new DataTable();
            
            dataTable.Columns.Add(column);

            List<DataRow> rows = new List<DataRow>();
            if (list == null)
            {
                return dataTable;
            }
            int rowCount = list.Count;
            for (int i = 0; i < rowCount; i++)
            {
                rows.Add(dataTable.NewRow());
            }
            int j = 0;
            foreach (DataRow row in rows)
            {
                row[columnName] = list.ElementAt(j);
                dataTable.Rows.Add(row);
                j++;
            }

            return dataTable;
        }
    }
}