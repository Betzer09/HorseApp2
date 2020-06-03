using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HorseApp2.Versions.v1_1.Models;
using Microsoft.Web.Http;

namespace HorseApp2.Versions.v1_1.Controllers
{
    [ApiVersion("1.1")]
    public class DatabaseHelper
    {
        #region Public Methods

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
            catch (Exception exception)
            {
                return new List<HorseListing>();
            }
        }
        
        /// <summary>
        /// Builds an active listings search request from the given data source
        /// </summary>
        /// <param name="headers">Data source to search for parameters</param>
        /// <returns>Initialized search active listings request from the given data source</returns>
        /// <exception cref="Exception">Rethrown exception from parameter builder functions</exception>
        public async Task<SearchActiveListingsRequest> BuildListingRequest(HttpHeaders headers)
        {
            var request = new SearchActiveListingsRequest();
            
            // Handle Simple Parameters
            (request.TypeSearch, request.HorseTypes) = CheckStringListParam(headers, "types"); 
            (request.SireSearch, request.Sires) = CheckStringListParam(headers, "sires");
            (request.GenderSearch, request.Genders) = CheckStringListParam(headers, "genders");
            (request.AgeSearch, request.Ages) = CheckIntListParam(headers, "ages", ' ');
            (request.DamSearch, request.Dams) = CheckStringListParam(headers, "dams");
            (request.DamSireSearch, request.DamSires) = CheckStringListParam(headers, "damSires");
            (request.ColorSearch, request.Colors) = CheckStringListParam(headers, "colors", ' ');
            (request.InFoalSearch, request.InFoal) = CheckBoolParam(headers, "inFoal");
            (request.ActiveListingIdSearch, request.ActiveListingIds) =
                CheckStringListParam(headers, "activeListingIds", ' ');
            (request.InFoalToSearch, request.InFoalTo) = CheckStringListParam(headers, "inFoalTo");
            (request.isSoldSearch, request.IsSold) = CheckBoolParam(headers, "isSold");
            (request.isRegisteredSearch, request.isRegistered) = CheckBoolParam(headers, "isRegistered");
            (request.HeightSearch, request.Heights) = CheckDoubleListParam(headers, "heights");
            (request.IsSireRegisteredSearch, request.IsSireRegistered) = CheckBoolParam(headers, "isSireRegistered");
            (request.IsDamSireRegisteredSearch, request.IsDamSireRegistered) =
                CheckBoolParam(headers, "isDamSireRegistered");
            request.ItemsPerPage = CheckIntParam(headers, "itemsPerPage", 20).Item2;
            request.Page = CheckIntParam(headers, "page", 1).Item2;
            request.OrderBy = CheckBoolParam(headers, "orderBy").Item2;
            request.OrderByType = CheckIntParam(headers, "orderByType", 1).Item2;
            request.OrderByDesc = CheckBoolParam(headers, "orderByDesc").Item2;
            
            // Handle Postal Code Metadata
            var (zipCodeExists, zipCode) = CheckStringParam(headers, "zip");
            var (countryCodeExists, countryCode) = CheckStringParam(headers, "countryCode");
            request.LocationsSearch = zipCodeExists && countryCodeExists;
            
            // Throw an error if zip code or country code are provided without the other.
            if (zipCodeExists && !countryCodeExists || countryCodeExists && !zipCodeExists)
            {
                throw new HttpRequestException("Postal and country codes must be provided together for location filtering.");
            }
            
            if (zipCodeExists)
            {
                request.CountryCode = countryCode;
                request.Range = CheckIntParam(headers, "dist", 25).Item2;
                request.Unit = CheckStringParam(headers, "unit", "mile").Item2;

                var geographyRequestController = new GeographyRequestController();
                geographyRequestController.ValidateParameters(request.Range, request.Unit);
                request.PostalCode = geographyRequestController.PreparePostalCode(zipCode, request.CountryCode);
            }

            // Handle Range Parameters
            var (priceSearch, priceMin, priceMax) = CheckDecimalRangeParam(headers, "priceLow", "priceHigh");
            if (priceSearch)
            {
                request.PriceSearch = true;
                request.PriceLow = priceMin;
                request.PriceHigh = priceMax;
            }
            
            var (lteSearch, lteMin, lteMax) = CheckDecimalRangeParam(headers, "lteLow", "lteHigh");
            if (lteSearch)
            {
                request.LteSearch = true;
                request.LteLow = lteMin;
                request.LteHigh = lteMax;
            }

            // Return the results
            return request;
        }
        
        /// <summary>
        /// Creates parameters for the stored procedure: usp_InsertAcitveListing given a horseListing
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        public List<SqlParameter> GetSqlParametersForInsert(HorseListing listing)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(BuildSqlParameter("@ActiveListingId", listing.activeListingId));
            parameters.Add(BuildSqlParameter("@Age", listing.age));
            parameters.Add(BuildSqlParameter("@Color", listing.color));
            parameters.Add(BuildSqlParameter("@Dam", listing.dam));
            parameters.Add(BuildSqlParameter("@Sire", listing.sire));
            parameters.Add(BuildSqlParameter("@DamSire", listing.damSire));
            parameters.Add(BuildSqlParameter("@Description", listing.description));
            parameters.Add(BuildSqlParameter("@Gender", listing.gender));
            parameters.Add(BuildSqlParameter("@HorseName", listing.horseName));
            parameters.Add(BuildSqlParameter("@InFoal", listing.inFoal));
            parameters.Add(BuildSqlParameter("@Lte", listing.lte));
            parameters.Add(BuildSqlParameter("@OriginalDateListed", listing.originalDateListed));
            parameters.Add(BuildSqlParameter("@Price", listing.price));
            parameters.Add(BuildSqlParameter("@PurchaseListingType", listing.purchaseListingType));
            parameters.Add(BuildSqlParameter("@RanchPhoto", listing.ranchPhoto));
            parameters.Add(BuildSqlParameter("@SellerId", listing.sellerId));
            parameters.Add(BuildSqlParameter("@HorseType", listing.horseType));
            parameters.Add(BuildSqlParameter("@IsSold", listing.isSold));
            parameters.Add(BuildSqlParameter("InFoalTo", listing.InFoalTo));
            parameters.Add(BuildSqlParameter("@CallForPrice", listing.callForPrice));
            parameters.Add(BuildSqlParameter("@Height", listing.Height));
            
            var geographyHelper = new GeographyRequestController();
            parameters.Add(BuildSqlParameter(
                "@Zip",
                geographyHelper.PreparePostalCode(listing.Zip, listing.CountryCode)));
            parameters.Add(BuildSqlParameter("@CountryCode", listing.CountryCode));

            return parameters;
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
            
            // Location Search Parameters
            var geographyHelper = new GeographyRequestController();
            parameters.Add(BuildSqlParameter("@LocationsSearch", request.LocationsSearch));
            parameters.Add(BuildSqlParameter(
                "@PostalCode",
                geographyHelper.PreparePostalCode(request.PostalCode, request.CountryCode)));
            parameters.Add(BuildSqlParameter("@Range", request.Range));
            parameters.Add(BuildSqlParameter("@CountryCode", request.CountryCode));
            parameters.Add(BuildSqlParameter("@Unit", request.Unit));
            
            return parameters;
        }
        
        /// <summary>
        /// Convenience function used to build a SQL Parameter
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Initialized SQL parameter</returns>
        public SqlParameter BuildSqlParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
        
        /// <summary>
        /// Populates an Active Listing object with data rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        public HorseListing PopulateListing(DataRow row, List<DataRow> photos)
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
            listing.Zip = row["Zip"].ToString();
            listing.CountryCode = row["CountryCode"].ToString();
            listing.ViewedCount = int.Parse(row["ViewedCount"].ToString());
            listing.FavoriteCount = int.Parse(row["FavoriteCount"].ToString());

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

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if the given parameter name exists as a list of strings in the source
        /// </summary>
        /// <remarks>Broken into a List of Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="separator">Separator used to split the given list</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, List<string>> CheckStringListParam(HttpHeaders headers, string parameterName,
            char separator = ',')
        {
            if (!headers.Contains(parameterName))
            {
                return new Tuple<bool, List<string>>(false, new List<string>());
            }
            
            var input = headers.GetValues(parameterName).First().Trim('{', '}', '[', ']').Replace("\"", "");
            var searchValue = input.Split(separator).ToList();
            for (int i = 0; i < searchValue.Count; i++)
            {
                searchValue[i] = searchValue[i].TrimStart(' ');
            }

            return new Tuple<bool, List<string>>(true, searchValue);
        }
        
        /// <summary>
        /// Checks if the given parameter name exists as a list of integers in the source
        /// </summary>
        /// <remarks>Broken into a List of Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="separator">Separator used to split the given list in its string form</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, List<int>> CheckIntListParam(HttpHeaders headers, string parameterName,
            char separator = ',')
        {
            var (exists, stringValues) = CheckStringListParam(headers, parameterName, separator);
            if (!exists)
            {
                return new Tuple<bool, List<int>>(false, new List<int>());
            }
            
            var results = new List<int>();
            foreach (var stringValue in stringValues)
            {
                var valid = int.TryParse(stringValue, out var value);
                if (valid)
                {
                    results.Add(value);
                }
            }

            return new Tuple<bool, List<int>>(true, results);
        }
        
        /// <summary>
        /// Checks if the given parameter name exists as a list of doubles in the source
        /// </summary>
        /// <remarks>Broken into a List of Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="separator">Separator used to split the given list in its string form</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, List<double>> CheckDoubleListParam(HttpHeaders headers, string parameterName,
            char separator = ',')
        {
            var (exists, stringValues) = CheckStringListParam(headers, parameterName, separator);
            if (!exists)
            {
                return new Tuple<bool, List<double>>(false, new List<double>());
            }
            
            var results = new List<double>();
            foreach (var stringValue in stringValues)
            {
                var valid = double.TryParse(stringValue, out var value);
                if (valid)
                {
                    results.Add(value);
                }
            }

            return new Tuple<bool, List<double>>(true, results);
        }

        /// <summary>
        /// Checks if the given parameter name exists as a string in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, string> CheckStringParam(HttpHeaders headers, string parameterName, string defaultValue = "")
        {
            if (!headers.Contains(parameterName))
            {
                return new Tuple<bool, string>(false, defaultValue);
            }

            var param = headers.GetValues(parameterName).First();
            return new Tuple<bool, string>(true, param);
        }

        /// <summary>
        /// Checks if the given parameter name exists as an integer in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, int> CheckIntParam(HttpHeaders headers, string parameterName, int defaultValue = 0)
        {
            var (exists, stringValue) = CheckStringParam(headers, parameterName);
            if (!exists)
            {
                return new Tuple<bool, int>(false, defaultValue);
            }
            
            var valid = int.TryParse(stringValue, out var value);
            return new Tuple<bool, int>(true, valid ? value : defaultValue);
        }
        
        /// <summary>
        /// Checks if the given parameter name exists as a double in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, double> CheckDoubleParam(HttpHeaders headers, string parameterName, double defaultValue = 0)
        {
            var (exists, stringValue) = CheckStringParam(headers, parameterName);
            if (!exists)
            {
                return new Tuple<bool, double>(false, defaultValue);
            }
            
            var valid = double.TryParse(stringValue, out var value);
            return new Tuple<bool, double>(true, valid ? value : defaultValue);
        }
        
        /// <summary>
        /// Checks if the given parameter name exists as a decimal in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, decimal> CheckDecimalParam(HttpHeaders headers, string parameterName, decimal defaultValue = 0)
        {
            var (exists, stringValue) = CheckStringParam(headers, parameterName);
            if (!exists)
            {
                return new Tuple<bool, decimal>(false, defaultValue);
            }
            
            var valid = decimal.TryParse(stringValue, out var value);
            return new Tuple<bool, decimal>(true, valid ? value : defaultValue);
        }
        
        /// <summary>
        /// Checks if the given parameter name exists as a boolean in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, bool> CheckBoolParam(HttpHeaders headers, string parameterName, bool defaultValue = false)
        {
            var (exists, stringValue) = CheckStringParam(headers, parameterName);
            if (!exists)
            {
                return new Tuple<bool, bool>(false, defaultValue);
            }
            
            var valid = bool.TryParse(stringValue, out var value);
            return new Tuple<bool, bool>(true, valid ? value : defaultValue);
        }

        /// <summary>
        /// Checks if the given parameter name exists as a decimal in the source
        /// </summary>
        /// <remarks>Broken into a Type version instead of templates to avoid unnecessary complexity and risk with type comparisons</remarks>
        /// <param name="headers">Parameter source</param>
        /// <param name="parameterName">Parameter to check for</param>
        /// <param name="defaultValue">Default value in case one does not exist</param>
        /// <returns>Tuple listing whether the given parameter exists and the associated value</returns>
        private Tuple<bool, decimal, decimal> CheckDecimalRangeParam(HttpHeaders headers, string minParamName, string maxParamName,
            decimal minDefault = 0, decimal maxDefault = 10000000)
        {
            var exists = headers.Contains(minParamName) || headers.Contains(maxParamName);

            var min = CheckDecimalParam(headers, minParamName, minDefault);
            var max = CheckDecimalParam(headers, maxParamName, maxDefault);
            return new Tuple<bool, decimal, decimal>(exists, min.Item2, max.Item2);
        }

        /// <summary>
        /// Convenience function used to build a SQL Parameter
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <param name="type">DB Type of the parameter</param>
        /// <returns>Initialized SQL parameter</returns>
        private SqlParameter BuildSqlParameter(string name, object value, SqlDbType type)
        {
            var param = BuildSqlParameter(name, value);
            param.SqlDbType = type;
            return param;
        }

        /// <summary>
        /// Convenience function used to build the Horse Type parameter for database requests
        /// </summary>
        /// <param name="horseTypes">List of horse types to include in the search request</param>
        /// <returns>Data table representing the list of horse types</returns>
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

        /// <summary>
        /// Convenience function used to build out SQL parameter values for database requests from a list
        /// </summary>
        /// <param name="list">List of values to assign to the parameter</param>
        /// <param name="columnName">Name of the database type column</param>
        /// <param name="typeName">Database system type to associate to the parameter</param>
        /// <typeparam name="T">Type of the associated list</typeparam>
        /// <returns>Data table representing the given list</returns>
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

        #endregion
        
    }
}