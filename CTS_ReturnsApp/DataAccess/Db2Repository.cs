using CTS_ReturnsApp.Interfaces;
using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public List<IssueSTlModelSimple> GetIssuesShopIssueShoptechList(DateTime first, DateTime second, string vin)
        {

            var result = new List<IssueSTlModelSimple>();
            List<string> vins = new List<string>
            {
                vin.ToUpper()
            };
            var ListIssuesShopIssueShoptech = GetIssuesShopIssueShoptechLst(first, second, vin);

            return ListIssuesShopIssueShoptech;
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
                        endMFG = registros["DATE_MFG_RLSE"] == DBNull.Value ? DateTime.Today : DateTime.Parse(DateTime.Parse(registros["DATE_MFG_RLSE"].ToString().Trim()).ToString("yyyy-MM-dd")),
                        startChasis = registros["DATE_CHASSIS_START"] == DBNull.Value ? DateTime.Today : DateTime.Parse(DateTime.Parse(registros["DATE_CHASSIS_START"].ToString().Trim()).ToString("yyyy-MM-dd")),
                        vin = registros["VEH_SER_NO"].ToString().Trim()
                    });
                }
                cnx.Close();
                //mail.SendMailError("Si llega hasta después de checar los datos en DB2 peggo", new Exception("Probando"));
                return list_return;
            }
        }

        public List<IssueSTlModelSimple> GetIssuesShopIssueShoptechLst(DateTime first, DateTime second, string vin)
        {
            GeneralUtlts UtilitiesServ  = new GeneralUtlts();
            //first = UtilitiesServ.ConvertSaltilloTimeToPorlantByDate(first);
            //second = second;
            List<IssueSTlModelSimple> issues = new List<IssueSTlModelSimple>();
            try
            {
                //cnx.Open();
               var query = @"SELECT                    
                              PRODDB2.QAVHITDS.FOUND_INSP_TEAM --Encontrado por  
                            , PRODDB2.QAVHITDS.RESP_INSP_TEAM --Encontrado por  
                            , PRODDB2.QAVHITDS.INSP_ITEM --ItemID  
                            , PRODDB2.QAINITEM.INSP_ITEM_DESC --ItemDesc
                            , PRODDB2.QAVHITDS.INSP_DSCREP--Discrepancia
                            , PRODDB2.QAVHITDS.INSP_DSCREP_DESC--Discrepancia
                            , PRODDB2.QAVHITDS.INSP_COMT --Comentario
                            , PRODDB2.QAVHITDS.INSP_ID --Acceso Por 
                            , PRODDB2.QAVHITDS.TS_LOAD --Carga Inicial 
                            , PRODDB2.QAVHITDS.VEH_SER_NO --VIN
                            ,'NO' as YES
                        FROM 
                            PRODDB2.QAVHITDS SI
                           INNER JOIN  PRODDB2.QAINITEM IT
                          ON PRODDB2.QAINITEM.INSP_ITEM = PRODDB2.QAVHITDS.INSP_ITEM 
                        WHERE
                            PRODDB2.QAVHITDS.LOC = 013
                            AND PRODDB2.QAVHITDS.VEH_SER_NO='" + vin.ToUpper() + @"'
                            AND PRODDB2.QAVHITDS.TS_LOAD BETWEEN '" + first.ToString("yyyy-MM-dd-HH.mm.ss.ffffff000") + @"' AND '" + second.ToString("yyyy-MM-dd-HH.mm.ss.ffffff000") + @"' 
                            AND ( PRODDB2.QAVHITDS.FOUND_INSP_TEAM = 711 OR PRODDB2.QAVHITDS.FOUND_INSP_TEAM = 718) AND RPAR_ID IS NULL;";

                using (OleDbConnection cnx = new OleDbConnection(_connectionString))
                {
                    cnx.Open();
                    OleDbCommand cmd = new OleDbCommand(query, cnx);
                    OleDbDataReader registros = cmd.ExecuteReader();
                    while (registros.Read())
                    {
                        IssueSTlModelSimple art = new IssueSTlModelSimple
                        {
                            item_desc = registros["INSP_ITEM"].ToString() + " " + registros["INSP_ITEM_DESC"].ToString(),
                            ts_load = UtilitiesServ.ConvertPortlandTimeToSaltilloByDate(DateTime.Parse(registros["TS_LOAD"].ToString())),
                            found_team = registros["FOUND_INSP_TEAM"].ToString(),
                            veh_ser_no = registros["VEH_SER_NO"].ToString(),
                            resp_team = registros["RESP_INSP_TEAM"].ToString(),
                            disc_desc = registros["INSP_DSCREP"].ToString() +" "+registros["INSP_DSCREP_DESC"].ToString(),
                            found_person = registros["INSP_ID"].ToString(),
                            comment = registros["INSP_COMT"].ToString(),
                        };
                        issues.Add(art);
                    }

                    string ids = "0";
                for (int i = 0; i < issues.Count; i++)
                {
                    ids += "," + issues[i].item_desc;
                }
       

               
            }
            }
            catch (Exception ex)
            {
                //Email e = new Email();
                //e.SendErrorMail("", ex, cmd.CommandText.ToString());
            }
            finally
            {
              
            }
            return issues;
        }


    }
}
