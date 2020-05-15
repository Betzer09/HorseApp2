using System;
using System.Collections.Generic;
using System.Data;
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
    }
}