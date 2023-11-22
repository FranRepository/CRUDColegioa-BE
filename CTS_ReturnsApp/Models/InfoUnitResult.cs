namespace CTS_ReturnsApp.Models
{
    public class InfoUnitResult
    {

        public DateTime STARTCHASISDATE { get; set; }
        public DateTime ENDMFGDATE { get; set; }

        public string? CUST { get; set; }
        public string? VIN { get; set; }
    }

    public class InfoUnit
    {
        public string? VIN { get; set; }
    }
}
