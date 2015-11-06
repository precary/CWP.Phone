namespace CWP.Phone
{
    public interface IPhoneNumber
    {
        string CountryCode { get; set; }
        string Number { get; set; }
    }
}