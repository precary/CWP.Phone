namespace CWP.Phone
{
    static class StringExtensions
    {
        public static string TrimSafely(this string value)
        {
            return value == null ? string.Empty : value.Trim();
        }
    }
}