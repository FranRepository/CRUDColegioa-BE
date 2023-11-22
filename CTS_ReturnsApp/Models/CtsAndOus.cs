namespace CTS_ReturnsApp.Models
{
    public class CtsAndOus
    {
        public CtsReturns? CtsReturns { get; set; }
        public List<CtsOu>? CtsOu { get; set; }

        public UserData? UserData { get; set; }

    }

    public class CtsAndOuAcceptOrDeclineRequest
    {
        public int ItemId { get; set; }

        public string VIN { get; set; }

        public List<CtsOu>? CtsOu { get; set; }

        public UserData? UserData { get; set; }
        public bool AcceptedOrDeclined { get;}

    }
}

