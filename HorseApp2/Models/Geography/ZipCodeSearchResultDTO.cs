namespace HorseApp2.Models.Geography
{
    /// <summary>
    /// Core result sub-object format from the third-party API
    /// </summary>
    public struct ZipCodeSearchRequestResultDto
    {
        public string zip_code;
        public string city;
        public string state;
        public float distance;
    }

    /// <summary>
    /// Result array format from the third-party API
    /// </summary>
    public struct ZipCodeSearchRequestResultsDto
    {
        public ZipCodeSearchRequestResultDto[] zip_codes;
    }

}