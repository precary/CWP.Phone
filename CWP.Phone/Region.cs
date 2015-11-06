namespace CWP.Phone
{
    class Region
    {
        readonly string regionName;
        readonly string countryCode;

        internal Region(string regionName, string countryCode)
        {
            this.regionName = regionName;
            this.countryCode = countryCode;
        }

        public string RegionName { get { return regionName; } }
        public string CountryCode { get { return countryCode; } }
    }
}