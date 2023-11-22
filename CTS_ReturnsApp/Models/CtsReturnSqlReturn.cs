namespace CTS_ReturnsApp.Models
{
    public class CtsReturnSqlReturn
    {
        public int itemid { get; set; }
        public string? vin { get; set; }
        public string? customer { get; set; }
        public int days_cts { get; set; }
        public int total_days_cts { get; set; }
        public string? discrepancy { get; set; }
        public bool material { get; set; }
        public List<int>? ou_resp_id { get; set; }
        public List<int>? ous_resp_all { get; set; }
        public string? ou_res_name_pdtes { get; set; }
        public string? ou_res_name_all { get; set; }
        public string? release_mfg_date { get; set; }
        public string? start_chasis_date { get; set; }
        public string? start_cts { get; set; }
        public string? requested_cts { get; set; }
        public string? user { get; set; }
        public bool active { get; set; }
        public int status { get; set; }
        public string? materials_req { get; set; }
        public string? eta_comment_cts { get; set; }
        public string? eta_comment_cts_repair { get; set; }
        public string? comment_repair { get; set; }
        public string? item_and_code { get; set; }
        //   public string discrepancie_and_code { get; set; }
        public string? ou_finder { get; set; }
        public string? users_repair_ou { get; set; }
        public string? user_accept_repair { get; set; }

        public string? user_accept_ingress { get; set; }
        public bool pagination { get; set; }


    }
}
