using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CWP.Phone
{
    static class Regions
    {
        static readonly Dictionary<string, Region> regionsMetadata;
        static readonly IList<KeyValuePair<string, string>> countryCodeOptions;

        static Regions()
        {
            regionsMetadata = CreateMetadata();
            countryCodeOptions = regionsMetadata
                .Values
                .Select(r => GetCountryCodeOption(r.RegionName))
                .OrderBy(r => r.Value)
                .ToList();
        }

        static KeyValuePair<string, string> GetCountryCodeOption(string regionCode)
        {
            var prefixBuilder = new StringBuilder()
                .Append(GetRegionCodeForDisplay(regionCode)).Append(" (+")
                .Append(GetCountryCode(regionCode))
                .Append(")");
            return new KeyValuePair<string, string>(
                regionCode,
                prefixBuilder.ToString());
        }

        static string GetRegionCodeForDisplay(string regionCode)
        {
            return regionCode == "GB" ? "UK" : regionCode;
        }

        static Dictionary<string, Region> CreateMetadata()
        {
            var metadata = XDocument.Parse(Embedded.PhoneNumberMetadata);
            return metadata.Descendants("territory")
                .Select(ParseRegion)
                .Where(r => r != null)
                .ToDictionary(r => r.RegionName, r => r);
        }

        static Region ParseRegion(XElement territory)
        {
            string id = GetAttribute(territory, "id");
            if (!IsValidRegion(id)) return null;
            string countryCode = GetAttribute(territory, "countryCode");
            if (countryCode == null) return null;
            return new Region(id, countryCode);
        }

        static bool IsValidRegion(string id)
        {
            if (id == null) return false;
            if (id.Length != 2) return false;
            return id.All(c => c >= 'A' && c <= 'Z');
        }

        static string GetAttribute(XElement territory, XName name)
        {
            var idAttribute = territory.Attribute(name);
            if (idAttribute == null) return null;
            return idAttribute.Value.TrimSafely();
        }

        public static bool IsSupportedRegion(string regionCode)
        {
            return regionsMetadata.ContainsKey(regionCode);
        }

        public static string GetCountryCode(string regionName)
        {
            Region region = regionsMetadata[regionName];
            return region.CountryCode;
        }

        public static IList<KeyValuePair<string, string>> GetCountryCodeOptions()
        {
            return countryCodeOptions;
        }
    }
}