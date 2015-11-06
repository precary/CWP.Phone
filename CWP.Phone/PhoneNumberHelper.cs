using System;
using System.Collections.Generic;
using System.Text;

namespace CWP.Phone
{
    public static class PhoneNumberHelper
    {
        public static T ParsePhoneNumber<T>(string phoneNumber)
            where T : IPhoneNumber, new()
        {
            string normalizedPhone = phoneNumber.TrimSafely();
            if (string.IsNullOrEmpty(normalizedPhone)) return new T();
            string regionName = GetRegion(normalizedPhone);
            string number = regionName == null ? null : GetNumber(normalizedPhone, regionName);

            bool isValid = regionName != null && number != null;

            return new T
            {
                CountryCode = isValid ? regionName : string.Empty,
                Number = isValid ? number : phoneNumber
            };
        }

        public static string ToRegionE164PhoneNumber(IPhoneNumber phoneNumber)
        {
            ValidationResult errorType;
            Exception error;
            string result = InternalConvertToRegionE164PhoneNumber(phoneNumber, out errorType, out error);
            if (errorType != ValidationResult.Success)
            {
                throw error;
            }

            return result;
        }

        public static string TryConvertToRegionE164PhoneNumber(
            IPhoneNumber phoneNumber,
            out ValidationResult errorType)
        {
            Exception error;
            return InternalConvertToRegionE164PhoneNumber(phoneNumber, out errorType, out error);
        }

        static string InternalConvertToRegionE164PhoneNumber(
            IPhoneNumber phoneNumber,
            out ValidationResult errorType,
            out Exception error)
        {
            errorType = ValidationResult.Success;
            error = null;

            if (IsEmpty(phoneNumber))
            {
                return string.Empty;
            }

            string regionName = phoneNumber.CountryCode;
            string number = RemovePunctuation(phoneNumber.Number);

            if (!Regions.IsSupportedRegion(regionName) || string.IsNullOrEmpty(number))
            {
                errorType = ValidationResult.InvalidNumber;
                error = new FormatException(
                    string.Format(
                        "Invalid country code or empty number: {0}|{1}",
                        phoneNumber.CountryCode,
                        phoneNumber.Number));
                return null;
            }

            return regionName + "|" + GetCountryCode(regionName) + number;
        }

        static string RemovePunctuation(string nationalNumber)
        {
            if (string.IsNullOrEmpty(nationalNumber)) return nationalNumber;
            int length = nationalNumber.Length;
            var numberBuilder = new StringBuilder(length);
            foreach (char c in nationalNumber)
            {
                if (c >= '0' && c <= '9') numberBuilder.Append(c);
            }

            return numberBuilder.ToString();
        }

        public static string ToParsableString(IPhoneNumber phoneNumber)
        {
            try
            {
                return ToRegionE164PhoneNumber(phoneNumber);
            }
            catch
            {
                return phoneNumber.Number ?? string.Empty;
            }
        }

        public static IList<KeyValuePair<string, string>> GetCountryCodeMap()
        {
            return Regions.GetCountryCodeOptions();
        }

        static string GetCountryCode(string regionName)
        {
            if (!Regions.IsSupportedRegion(regionName))
            {
                return string.Empty;
            }

            return new StringBuilder(16)
                .Append("+")
                .Append(Regions.GetCountryCode(regionName)).ToString();
        }

        static bool IsEmpty(IPhoneNumber phoneNumber)
        {
            return string.IsNullOrEmpty(phoneNumber.CountryCode) &&
                string.IsNullOrEmpty(phoneNumber.Number);
        }

        public static string GetRegion(string phoneNumber)
        {
            string normalizedPhone = phoneNumber.TrimSafely();
            if (string.IsNullOrEmpty(normalizedPhone)) return null;
            int hintSeparator = normalizedPhone.IndexOf('|');
            if (hintSeparator == -1) return null;
            string regionHint = normalizedPhone.Substring(0, hintSeparator);
            if (Regions.IsSupportedRegion(regionHint))
            {
                return regionHint;
            }

            return null;
        }

        static string GetNumber(string phoneNumber, string regionCode)
        {
            if (!Regions.IsSupportedRegion(regionCode)) return null;
            string normalizedPhone = phoneNumber.TrimSafely();
            if (string.IsNullOrEmpty(normalizedPhone)) return null;
            int hintSeparator = normalizedPhone.IndexOf('|');
            if (hintSeparator == -1) return null;
            if (hintSeparator == normalizedPhone.Length - 1) return null;
            string internationalNumber = normalizedPhone.Substring(
                hintSeparator + 1,
                normalizedPhone.Length - hintSeparator - 1);
            if (!internationalNumber.StartsWith("+")) return null;
            return internationalNumber.Replace("+" + Regions.GetCountryCode(regionCode), "");
        }
    }
}