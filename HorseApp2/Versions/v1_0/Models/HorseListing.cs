﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Web.Http;
using Newtonsoft.Json;

namespace HorseApp2.Versions.v1_0.Models
{
    //1 horse listing that includes the photos for the post
    [ApiVersion("1.0")]
    public class HorseListing
    {
        public HorseListing()
        {
            photos = new List<HorseListingPhoto>();
        }

        public string activeListingId { get; set; }
        public int? age { get; set; }
        public string color { get; set; }
        public string dam { get; set; }
        public string sire { get; set; }
        public string damSire { get; set; }
        public string description { get; set; }
        public string gender { get; set; }
        public string horseName { get; set; }
        public bool inFoal { get; set; }
        public decimal lte { get; set; }
        public string originalDateListed { get; set; }
        public decimal price { get; set; }
        public string purchaseListingType { get; set; }
        public string ranchPhoto { get; set; }
        public string sellerId { get; set; }
        public string horseType { get; set; }
        public bool isSold { get; set; }
        public string InFoalTo { get; set; }
        public bool IsSireRegistered { get; set; }
        public bool IsDamSireRegistered { get; set; }
        public bool? callForPrice { get; set; }
        public double Height { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("favoriteCount")]
        public int FavoriteCount { get; set; }
        [JsonProperty("viewedCount")]
        public int ViewedCount { get; set; }

        public List<HorseListingPhoto> photos { get; set; }

    }
}