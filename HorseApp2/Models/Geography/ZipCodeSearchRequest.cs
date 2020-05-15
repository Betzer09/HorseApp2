namespace HorseApp2.Models.Geography
{
    public class ZipCodeSearchRequest
    {
        /// <summary>
        /// Zip code used as the focal point of the search. Required.
        /// </summary>
        public string OriginZipCode { get; set; }

        /// <summary>
        /// Integer distance representing the search radius. Optional.
        /// </summary>
        public int Distance { get; set; } = 50;

        /// <summary>
        /// The type of units to measure distance in. Valid options are "mile" and "km". Optional.
        /// </summary>
        public string Units { get; set; }
    }

    public class ZipCodeSearchRequestDTO
    {
        public string zip { get; set; }
        public string units { get; set; } = "mile";
        public int dist { get; set; }= 50;
    }

}