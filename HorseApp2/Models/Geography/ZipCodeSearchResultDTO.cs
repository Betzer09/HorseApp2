namespace HorseApp2.Models.Geography
{
    /// <summary>
    /// Core result sub-object format from the third-party API
    /// </summary>
    public struct ZipCodeSearchRequestResultDTO
    {
        public string zip_code;
        public string city;
        public string state;
        public float distance;
    }

    /// <summary>
    /// Result array format from the third-party API
    /// </summary>
    public struct ZipCodeSearchRequestResultsDTO
    {
        public ZipCodeSearchRequestResultDTO[] zip_codes;
    }

    /// <summary>
    /// Response sub-object sent to the requesting client from this API
    /// </summary>
    public struct ZipCodeSearchResponseDTO
    {
        public string zipCode;
        public string city;
        public string state;
        public float distance;
    }

    /// <summary>
    /// Response array sent to the requesting client from this API
    /// </summary>
    public struct ZipCodeSearchResponsesDTO
    {
        public ZipCodeSearchResponseDTO[] zipCodes;
    }
}