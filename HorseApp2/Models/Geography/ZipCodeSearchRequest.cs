namespace HorseApp2.Models.Geography
{
    /// <summary>
    /// Zip code search object following C# standards
    /// </summary>
    public class ZipCodeSearchRequest
    {
        /// <summary>
        /// Zip code used as the focal point of the search. Required.
        /// </summary>
        public string OriginZipCode { get; set; }

        /// <summary>
        /// Integer distance representing the search radius. Optional.
        /// </summary>
        public int Distance { get; set; } = 25;

        /// <summary>
        /// The type of units to measure distance in. Valid options are "mile" and "km". Optional.
        /// </summary>
        public string Units { get; set; } = "mile";
    }

    /// <summary>
    /// Zip code search Data Transfer Object (DTO) used to get parameters from client requests
    /// </summary>
    public class ZipCodeSearchRequestDto
    {
        /// <summary>
        /// Zip code used as the focal point of the search. Required.
        /// </summary>
        public string zip { get; set; }
        /// <summary>
        /// Integer distance representing the search radius. Optional.
        /// </summary>
        public string units { get; set; } = "mile";
        /// <summary>
        /// The type of units to measure distance in. Valid options are "mile" and "km". Optional.
        /// </summary>
        public int dist { get; set; } = 50;
    }

}