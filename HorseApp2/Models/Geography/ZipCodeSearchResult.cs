namespace HorseApp2.Models.Geography
{
    /// <summary>
    /// Search result for the zip code request endpoint.
    /// </summary>
    public class ZipCodeSearchResult
    {
        /// <summary>
        /// The result's postal (zip) code
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// The city the result is in
        /// </summary>
        public string City { get; set; }
        
        /// <summary>
        /// The abbreviated state code the result is in (e.g. "UT")
        /// </summary>
        public string State { get; set; }
        
        /// <summary>
        /// The distance from the requested zip code, rounded to 3 decimal points
        /// </summary>
        public float Distance { get; set; }

        /// <summary>
        /// Default constructor for search result for the zip code request endpoint.
        /// </summary>
        /// <param name="zipCode">The result's postal (zip) code</param>
        /// <param name="city">The city the result is in</param>
        /// <param name="state">The abbreviated state code the result is in (e.g. "UT")</param>
        /// <param name="distance">The distance from the requested zip code, rounded to 3 decimal points</param>
        public ZipCodeSearchResult(string zipCode, string city, string state, float distance)
        {
            ZipCode = zipCode;
            City = city;
            State = state;
            Distance = distance;
        }

        /// <summary>
        /// Constructor to build ZipCodeSearchResult from the third-party API's DTO
        /// </summary>
        /// <param name="dto">Data Transfer Object to convert</param>
        public ZipCodeSearchResult(ZipCodeSearchRequestResultDTO dto)
        {
            ZipCode = dto.zip_code;
            City = dto.city;
            State = dto.state;
            Distance = dto.distance;
        }

        /// <summary>
        /// Converts the ZipCodeSearchResult object into a DTO to send to the requesting client
        /// </summary>
        /// <returns>Outgoing DTO</returns>
        public ZipCodeSearchResponseDTO ToDto()
        {
            return new ZipCodeSearchResponseDTO()
            {
                zipCode = ZipCode,
                city = City,
                state = State,
                distance = Distance
            };
        }
    }
}