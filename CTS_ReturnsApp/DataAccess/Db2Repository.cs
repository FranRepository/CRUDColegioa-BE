using CTS_ReturnsApp.Interfaces;
using CTS_ReturnsApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;

namespace CTS_ReturnsApp.DataAccess
{
    public class Db2Repository : Repository<object>, IDb2Repository
    {
        public Db2Repository(string connectionString) : base(connectionString) { }

        public JsonResult InfoTruck(string vin)
        {

            var result = new JsonResult("");
            List<string> vins = new List<string>
            {
                vin.ToUpper()
            };
            var datosUnidad = InfoTruck(vins);



            if (datosUnidad.Any())
            {
                result = new JsonResult(new { STARTCHASISDATE = datosUnidad[0].startChasis.ToString(), ENDMFGDATE = datosUnidad[0].endMFG.ToString(), CUST = datosUnidad[0].customer.ToString(), VIN = datosUnidad[0].vin.ToString() });
            }

            return result;
        }

       private List<DataOMTExcel> InfoTruck(List<string> vins)
        {
            List<DataOMTExcel> data = GetDataFromDB2(vins);
            return data;
        }


        public List<DataOMTExcel>? GetDataFromDB2(List<string> vins)
        {
            string query = @"SELECT
                                SV.VEH_SER_NO
	                            ,CAST(SV.TS_LOAD AT TIME ZONE '-06:00' AS TIMESTAMP) AS SAL_TS_LOAD
                                , CASE
                                    WHEN TIME(TS_LOAD) BETWEEN '00:00:00' AND '04:25:00' THEN 3.1
                                    WHEN TIME(TS_LOAD) BETWEEN '04:25:00' AND '12:25:00' THEN 1
                                    WHEN TIME(TS_LOAD) BETWEEN '12:25:00' AND '19:55:00' THEN 2
                                    ELSE 3.2
                                END AS SHIFT
	                            ,SS.DATE_MFG_RLSE
	                            ,SS.DATE_CHASSIS_START
	                            ,SS.CUST_NAME_ABBR
                            FROM
                                PRODDB2.SV_PLANT_ACTY_DTL AS SV
                            LEFT JOIN
                                PRODDB2.VW_SDELVVEH_DPIMS AS SS
                                ON SS.VEH_SER_NO = SV.VEH_SER_NO
                                AND SS.LOC = SV.LOC
                            LEFT JOIN
                                (
                                    SELECT
                                        SKS.VEH_SER_NO,
                                        MAX(CASE WHEN DB_CD LIKE '980-%' THEN SUBSTR((RTRIM(VWSTSMOD.DB_SLS_DESC1)),13,LENGTH(RTRIM(VWSTSMOD.DB_SLS_DESC1)) - 12) END) AS M980_CODE_COLOR,
                                          MAX(CASE WHEN DB_CD LIKE '980-%' THEN(RTRIM(VWSTSMOD.DB_SLS_DESC2)) END) AS M980_DESCRIPTION_COLOR,
                                         MAX(CASE WHEN DB_CD LIKE '986-%' THEN SUBSTR((RTRIM(VWSTSMOD.DB_SLS_DESC1)), 16) END) AS M986_CODE_COLOR_CHASSIS,
                                          MAX(CASE WHEN DB_CD LIKE '986-%' THEN(RTRIM(VWSTSMOD.DB_SLS_DESC2)) END) AS M986_DESCRIPTION_COLOR_CHASSIS
                                    FROM
                                        PRODDB2.SV_PLANT_ACTY_DTL AS SV
                                    INNER JOIN
                                        PRODDB2.SKSPLIT_DPIMS AS SKS
                                        ON SV.VEH_SER_NO = SKS.VEH_SER_NO
                                    INNER JOIN PRODDB2.VW_STSOMOD_DPIMS AS  VWSTSMOD
                                        ON SKS.TSO_SPLIT_NO = VWSTSMOD.TSO_SPLIT_NO
                                    WHERE
                                        SV.LOC = 013
                                        AND SV.POOL_CD = '03'
                                        AND SKS.VEH_SER_NO IN('" + String.Join("', '", vins) + @"')
                                    GROUP BY
                                        SKS.VEH_SER_NO
	                            )AS P
                                ON P.VEH_SER_NO = SV.VEH_SER_NO
                            WHERE
                                SV.LOC = 013
                                AND SV.POOL_CD = 'DL'
                                AND SV.VEH_SER_NO IN('" + String.Join("', '", vins) + "') FOR FETCH ONLY WITH UR";
            using (OleDbConnection cnx = new OleDbConnection(_connectionString))
            {
                cnx.Open();
                OleDbCommand cmd = new OleDbCommand(query, cnx);
                OleDbDataReader registros = cmd.ExecuteReader();
                List<DataOMTExcel> list_return = new List<DataOMTExcel>();
                while (registros.Read())
                {

                    list_return.Add(new DataOMTExcel
                    {
                        customer = registros["CUST_NAME_ABBR"] == DBNull.Value || registros["CUST_NAME_ABBR"].ToString().Trim() == "" ? "" : registros["CUST_NAME_ABBR"].ToString().Trim(),
                        endMFG = registros["DATE_MFG_RLSE"] == DBNull.Value ? DateTime.Today : DateTime.Parse(registros["DATE_MFG_RLSE"].ToString().Trim()),
                        startChasis = registros["DATE_CHASSIS_START"] == DBNull.Value ? DateTime.Today : DateTime.Parse(registros["DATE_CHASSIS_START"].ToString().Trim()),
                        vin = registros["VEH_SER_NO"].ToString().Trim()
                    });
                }
                cnx.Close();
                //mail.SendMailError("Si llega hasta después de checar los datos en DB2 peggo", new Exception("Probando"));
                return list_return;
            }
        }

      
     
        
    }
}
