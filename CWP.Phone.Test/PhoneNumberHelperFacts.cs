using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Extensions;

namespace CWP.Phone.Test
{
    public class PhoneNumberHelperFacts
    {
        [Theory]
        [InlineData("XX", "15555555555")]
        [InlineData("CN", "")]
        public void should_throw_when_convert_invalid_phone_number(string countryCode, string number)
        {
            IPhoneNumber phoneNumber = new PhoneNumberImpl
            {
                CountryCode = countryCode,
                Number = number
            };

            AssertHelper.Throws(
                () => PhoneNumberHelper.ToRegionE164PhoneNumber(phoneNumber));            
        }

        [Fact]
        public void should_get_success_when_phone_number_is_empty()
        {
            IPhoneNumber phoneNumber = new PhoneNumberImpl
            {
                CountryCode = "",
                Number = ""
            };
            PhoneNumberHelper.ToRegionE164PhoneNumber(phoneNumber);

            Assert.DoesNotThrow(() => PhoneNumberHelper.ToRegionE164PhoneNumber(phoneNumber));
            Assert.Equal(string.Empty, PhoneNumberHelper.ToRegionE164PhoneNumber(phoneNumber));
        }
        [Theory]
        [InlineData("CN|+12341231234", "CN")]
        [InlineData("+12341231234", null)]
        [InlineData(" CN|+12341231234", "CN")]
        [InlineData("", null)]
        public void should_get_region_hint(string phoneNumber, string countryCode)
        {
            Assert.Equal(countryCode, PhoneNumberHelper.GetRegion(phoneNumber));
        }

        [Fact]
        public void should_display_UK_for_region_GB()
        {
            IList<KeyValuePair<string, string>> codeMap = PhoneNumberHelper.GetCountryCodeMap();
            string displayedRegionCode = codeMap.Single(c => c.Key == "GB").Value;

            Assert.Equal("UK (+44)", displayedRegionCode);
        }

        [Theory]
        [InlineData("CN|+8613888888888", "CN", "13888888888")]
        [InlineData("US|+16508923520", "US", "6508923520")]
        public void should_parse_correctly_formatted_number(
            string phoneNumber,
            string region,
            string nationalNumber)
        {
            var parsed = PhoneNumberHelper.ParsePhoneNumber<PhoneNumberImpl>(phoneNumber);
            Assert.Equal(region, parsed.CountryCode);
            Assert.Equal(nationalNumber, parsed.Number);
        }

        [Theory]
        [InlineData("CN|8613888888888", "", "CN|8613888888888")]
        [InlineData("KAO|+16508923520", "", "KAO|+16508923520")]
        public void should_parse_malformed_number(
            string phoneNumber,
            string region,
            string nationalNumber)
        {
            var parsed = PhoneNumberHelper.ParsePhoneNumber<PhoneNumberImpl>(phoneNumber);
            Assert.Equal(region, parsed.CountryCode);
            Assert.Equal(nationalNumber, parsed.Number);
        }

        [Theory]
        [InlineData("138 8888 8888", "CN|+8613888888888")]
        [InlineData("138-8888 8888", "CN|+8613888888888")]
        [InlineData("138\u20108888 8888", "CN|+8613888888888")]
        [InlineData("138-8888 8888", "CN|+8613888888888")]
        [InlineData("138\u20158888 8888", "CN|+8613888888888")]
        [InlineData("138\u22128888 8888", "CN|+8613888888888")]
        [InlineData("138\u30FC8888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF0D8888 8888", "CN|+8613888888888")]
        [InlineData("138-8888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF0F8888 8888", "CN|+8613888888888")]
        [InlineData("138\u00A08888 8888", "CN|+8613888888888")]
        [InlineData("138\u00AD8888 8888", "CN|+8613888888888")]
        [InlineData("138\u200B8888 8888", "CN|+8613888888888")]
        [InlineData("138\u20608888 8888", "CN|+8613888888888")]
        [InlineData("138\u30008888 8888", "CN|+8613888888888")]
        [InlineData("138(8888 8888", "CN|+8613888888888")]
        [InlineData("138)8888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF088888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF098888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF3B8888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF3D8888 8888", "CN|+8613888888888")]
        [InlineData("138.8888 8888", "CN|+8613888888888")]
        [InlineData("138\\8888 8888", "CN|+8613888888888")]
        [InlineData("138[8888 8888", "CN|+8613888888888")]
        [InlineData("138]8888 8888", "CN|+8613888888888")]
        [InlineData("138\\8888 8888", "CN|+8613888888888")]
        [InlineData("138/8888 8888", "CN|+8613888888888")]
        [InlineData("138~8888 8888", "CN|+8613888888888")]
        [InlineData("138\u20538888 8888", "CN|+8613888888888")]
        [InlineData("138\u223C8888 8888", "CN|+8613888888888")]
        [InlineData("138\uFF5E8888 8888", "CN|+8613888888888")]
        public void should_ignore_wildcard_characters_when_parsing(string nationalPhoneNumber, string expectation)
        {
            string serialized = PhoneNumberHelper.ToParsableString(
                new PhoneNumberImpl {CountryCode = "CN", Number = nationalPhoneNumber});

            Assert.Equal(expectation, serialized);
        }
    }
}