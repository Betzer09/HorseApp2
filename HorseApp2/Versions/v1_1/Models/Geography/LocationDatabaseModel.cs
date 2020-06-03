using Newtonsoft.Json;

namespace HorseApp2.Versions.v1_1.Models.Geography
{
    public class LocationDatabaseModel
    {
        /// <summary>
        /// The ISO (two-character) code of the country
        /// </summary>
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        /// <summary>
        /// The postal/zip code of the location
        /// </summary>
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        /// <summary>
        /// The latitude coordinate (e.g. 42.0012)
        /// </summary>
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        /// <summary>
        /// The longitude coordinate (e.g. 42.0012)
        /// </summary>
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
        /// <summary>
        /// The 1-digit level of coordinate accuracy. 1 is highest, 6 is lowest.
        /// </summary>
        [JsonProperty("accuracy")]
        public int Accuracy { get; set; }
    }
}