using CTS_ReturnsApp.DataAccess;
using CTS_ReturnsApp.Interfaces;
using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CTS_ReturnsApp.Controllers
{
    public class CtsReturnsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkDb2 _unitOfWorkDb2;
        private readonly MexicanaHoldsContext _MexicanaHoldsContext;

        [HttpPost]
        public JsonResult NewCts(CtsAndOus CtsAndOus)
        {


            if (CtsAndOus.CtsReturns.Itemid != 0)
            {
                return UpdateHold(CtsAndOus);
            }
            else
            {
                if (CtsAndOus.CtsReturns.Vin != "" && CtsAndOus.CtsReturns.Itemid == 0)
                {
                   

                    JsonResult JsonR = _unitOfWorkDb2.Db2.InfoTruck(CtsAndOus.CtsReturns.Vin);
                    string JsonStr = JsonConvert.SerializeObject(JsonR);
                    InfoUnitResult responseUnitInfo = JsonConvert.DeserializeObject<InfoUnitResult>(JsonStr);

                    if (JsonStr != "" && JsonStr != "\"\"")
                    {
                        CtsAndOus.CtsReturns.ReleaseMfgDate = responseUnitInfo.ENDMFGDATE;
                        CtsAndOus.CtsReturns.StartChasisDate = responseUnitInfo.STARTCHASISDATE;
                        CtsAndOus.CtsReturns.Customer = responseUnitInfo.CUST;
                    }
                }

                try
                {
                    CtsAndOus.CtsReturns.StatusCts = -1;


                CtsReturns new_cts = CtsAndOus.CtsReturns;
                _MexicanaHoldsContext.CtsReturns.Add(new_cts);
                int result_cts = _MexicanaHoldsContext.SaveChanges();

                DateTime today = DateTime.Now;
                

                    if (result_cts > 0)
                    {
                        foreach (CtsOu ou in CtsAndOus.CtsOu)
                        {
                                //Se agregan las ous

                                CtsOu new_OuCts = ou;
                                new_OuCts.CtsRetursId = _MexicanaHoldsContext.CtsReturns.OrderByDescending(x=>x.Itemid).Select(x=>x.Itemid).First();
                                _MexicanaHoldsContext.CtsReturns.Add(new_cts);

                                int resultOus = _MexicanaHoldsContext.SaveChanges();

                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            result = false,
                            message = "Error save new CTS"
                        });
                    }

                }
                catch (Exception ex)
                {

                    return Json(new

                    {
                        result = false,
                        message = "Error save new CTS",
                        exception = ex.ToString()
                    });
                }



                //if (hold_return.hold_type_id != 3)
                //{

                //    EMail em = new EMail();
                //    var mails = mexicana_Holds.mails.Where(ma => ma.title == "On hold by Saltillo Plant").FirstOrDefault();
                //    mails.mails += ",miguel_angel.rodriguez@daimler.com";
                //    string holdType = mexicana_Holds.typeHolds.Where(x => x.itemid == hold_return.hold_type_id).Select(x => x.description_hold.ToString()).FirstOrDefault();

                //    string bodyMail = "Favor de poner la siguiente unidad en Hold: " + Cts.CtsReturns.vin + "</br></br>";
                //    bodyMail += "Por el concepto de: " + holdType + "</br>";
                //    bodyMail += "Y la discrepancia: " + hold_return.discrepancy + "</br>";


                //    em.SendMail(bodyMail, mails.mails, "Holds - Solicitud de ingreso a Hold: " + Cts.CtsReturns.vin, Cts.CtsReturns.mail_user);
                //}
                //else
                //{
                //    string holdType = mexicana_Holds.typeHolds.Where(x => x.itemid == hold_return.hold_type_id).Select(x => x.description_hold.ToString()).FirstOrDefault();

                //    EMail em = new EMail();
                //    var mails = new mail();
                //    mails = mexicana_Holds.mails.Where(ma => ma.title == "On hold by Saltillo Plant").FirstOrDefault();
                //    mails.mails += "miguel_angel.rodriguez@daimler.com";
                //    string bodyMail = "Favor de poner la siguiente unidad en Hold: " + Cts.CtsReturns.vin + "</br></br>";
                //    bodyMail += "Por el concepto de: " + holdType + "</br>";
                //    bodyMail += "Y la discrepancia: " + hold_return.discrepancy + "</br>";


                //    em.SendMail(bodyMail, mails.mails, "Holds - Solicitud de ingreso a Hold: " + Cts.CtsReturns.vin, Cts.CtsReturns.mail_user);
                //}



                //log_history.SaveLog(new log_hold
                //{
                //    posix_timestamp = Convert.ToInt32(DateTime.Now.ToUnixTime()),
                //    table_log = "new hold Solicitud de ingreso a Hold",
                //    text_log = JsonConvert.SerializeObject(hold_return),
                //    user_log = Cts.CtsReturns.user
                //});

                //bool save_history = log_history.SaveHistory(new unit_history
                //{
                //    ou_actual = hold_return.ou_resp,
                //    user_history = hold_return.user,
                //    posix_timestamp = Convert.ToInt32(DateTime.Now.ToUnixTime()),
                //    vin = hold_return.vin.ToUpper(),
                //    notes = hold_return.discrepancy,
                //    itemid = 0
                //});

                //if (save_history) { Console.WriteLine("Si guardó historial"); }
                //else { Console.WriteLine("No guardó historial"); }

                return Json(new
                {
                    result = true,
                    message = "Save succeful new Hold",

                });
            }
        }
        [HttpPost]
        public JsonResult UpdateHold(CtsAndOus Cts)
        {

        }


    }
}
