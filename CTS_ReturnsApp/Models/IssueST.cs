namespace CTS_ReturnsApp.Models
{
    public class IssueST
    {
        public int QA_ITEM_DSCREP_ID { get; set; }
        public DateTime TS_LOAD { get; set; }
        public string FOUND_INSP_TEAM { get; set; }
        public string INSP_COMT { get; set; }
        public string VEH_SER_NO { get; set; }
        public string RESP_INSP_OPRUNT { get; set; }
        public string RESP_INSP_TEAM { get; set; }
        public string DSCREP_DCAPA_VALUE { get; set; }
        public string RPAR_ID { get; set; }
        public string APPR_ID { get; set; }
        public string YES { get; set; }
    }

    public class IssueSTlModelSimple
    {
        public string item_desc { get; set; }
        public DateTime ts_load { get; set; }
        public string found_team { get; set; }
        public string resp_team { get; set; }
        public string veh_ser_no { get; set; }

        public string disc_desc { get; set; }

        public string found_person { get; set; }

        public string comment { get; set; }
    }
}
