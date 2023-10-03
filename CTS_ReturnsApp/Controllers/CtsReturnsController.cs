
using CTS_ReturnsApp.DataAccess;
using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Omu.ValueInjecter.Utils;

namespace CTS_ReturnsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CtsReturnsController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkDb2 _unitOfWorkDb2;
        private readonly MexicanaHoldsContext _MexicanaHoldsContext;

        public CtsReturnsController(IUnitOfWork unitOfWork, IUnitOfWorkDb2 unitOfWorkDb2)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkDb2 = unitOfWorkDb2;
        }
        

        [HttpPost(Name = "NewCts")]
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

                    return Json( new
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
                    message = "Save succeful new CTS Return",

                });
            }
        }
        [HttpPost(Name = "UpdateHold")]
        public JsonResult UpdateHold(CtsAndOus Cts)
        {
           var CtstoUpdate= _MexicanaHoldsContext.CtsReturns.FirstOrDefault(x => x.Itemid == Cts.CtsReturns.Itemid);
            var CtsRetReturn = new CtsAndOus();
          

            CtstoUpdate.Itemid = Cts.CtsReturns.Itemid;
            CtstoUpdate.Discrepancy = Cts.CtsReturns.Discrepancy;
            CtstoUpdate.Material = Convert.ToBoolean(Cts.CtsReturns.Material);
            CtstoUpdate.CommentRepair = Cts.CtsReturns.CommentRepair;
            CtstoUpdate.EtaCommentCtsRepair = Convert.ToDateTime(Cts.CtsReturns.EtaCommentCtsRepair);
            CtstoUpdate.FinderBy = Cts.CtsReturns.FinderBy;
            DateTime today = DateTime.Now;

            try
            {
                var material_hold = _MexicanaHoldsContext.MaterialCts.Where(mat => mat.CtsRetursId == Cts.CtsReturns.Itemid).ToList();
                var OusSinModificar = _MexicanaHoldsContext.CtsOu.Where(x => x.CtsRetursId == Cts.CtsReturns.Itemid).OrderBy(x => x.Itemid).Select(x => x.OuId).ToList();
                var idsCtsOus = _MexicanaHoldsContext.CtsOu.Where(x => x.CtsRetursId == Cts.CtsReturns.Itemid).Select(x => x).ToList();
                int result_hold = 1;

                int[] listaOusSelected = Cts.CtsOu
                    .Select(x => x.Itemid)
                    .ToArray();


                if (idsCtsOus.Count > 0)
                {
                    int count = 0;
                    foreach (var item in idsCtsOus)
                    {
                        if (!listaOusSelected.Contains(item.OuId.Value))
                        {
                            _MexicanaHoldsContext.CtsOu.Remove(item);
                            count++;
                        }
                    }
                    if (count >= 0)
                    {
                        result_hold = _MexicanaHoldsContext.SaveChanges();
                    }
                }



                if (result_hold > 0)
                {
                
                    var OusSelected = Cts.CtsOu.OrderBy(x => x.Itemid);

                    int i = 0;
                    foreach (CtsOu ou in OusSelected)
                    {

                        if (ou.Itemid != 0)
                        {
                            listaOusSelected[i] = ou.Itemid;
                        }
                        else
                        {
                            listaOusSelected[i] = 0;
                        }
                        i++;
                    }



                    if (CtstoUpdate.StatusCts == 0)
                    {
                        bool isDifferent = false;


                        foreach (int ouOriginal in OusSinModificar)
                        {
                            if (!listaOusSelected.Contains(ouOriginal))
                            {
                                isDifferent = true;

                            }
                            else
                            {
                                foreach (int add in listaOusSelected)
                                {
                                    if (!OusSinModificar.Contains(add))
                                    {
                                        isDifferent = true;
                                    }

                                }
                            }
                        }

                        List<CtsOu> CtsOu = _MexicanaHoldsContext.CtsOu.Where(x => x.CtsRetursId == CtstoUpdate.Itemid).ToList();
                        string TextAdittionarEmail = "";
                        if (CtstoUpdate.StatusCts == 0)
                        {
                            int ReleaseCount = 0;
                            foreach (CtsOu cts_ou in CtsOu)
                            {
                                if (cts_ou.Active == false)
                                {
                                    ReleaseCount++;
                                }
                            }
                            if (ReleaseCount == CtsOu.Count())
                            {
                                isDifferent = true;
                                foreach (CtsOu cts_ou in CtsOu)
                                {
                                    if (cts_ou.Active == false)
                                    {
                                        cts_ou.Active = true;
                                        _MexicanaHoldsContext.SaveChanges();
                                    }
                                }

                                TextAdittionarEmail = "</br><b> En caso de haber liberado anteriormente favor de confirmar liberación:</br> liberando " +
                                    "nuevamente por Reasignación</b></br> ";
                            }
                        }

                        if (isDifferent)
                        {
                            //if (hold_return.hold_type_id != 3)
                            ////{
                            //EMail em = new EMail();
                            //var mails = mexicana_Holds2.mails.Where(ma => ma.title == "Released").FirstOrDefault();

                            //var ous1 = mexicana_Holds2.ous.Where(ous => OusSinModificar.Contains(ous.itemid)).Select(x => x.ou_name).ToList();
                            //var ous2 = mexicana_Holds2.ous.Where(ous => listaOusSelected.Contains(ous.itemid)).Select(x => x.ou_name).ToList();

                            //string OUsMails = string.Join(",", mexicana_Holds2.ous.Where(x => listaOusSelected.Contains(x.itemid)).Select(x => x.MAIL_TO.ToString()));

                            //mails.mails += ",miguel_angel.rodriguez@daimler.com";
                            //mails.mails += "," + OUsMails;
                            //string holdType = mexicana_Holds2.typeHolds.Where(x => x.itemid == updatedHold.hold_type_id).Select(x => x.description_hold.ToString()).FirstOrDefault();

                            //string bodyMail = "Se reasigno unidad en Hold, Unidad: " + updatedHold.vin + "</br></br>";
                            //bodyMail += "Por el concepto de: " + holdType + "</br>";
                            //bodyMail += "Y la discrepancia: " + updatedHold.discrepancy + "</br>";
                            //bodyMail += "OUs Anteriores: " + string.Join(",", ous1) + "</br>";
                            //bodyMail += "OUs A Quien se reasigno: " + string.Join(",", ous2) + " </br>";
                            //bodyMail += TextAdittionarEmail;
                            //em.SendMail(bodyMail, mails.mails, "Holds - Hold Request: " + holdPosted.vin, holdPosted.mail_user);
                            //}
                        }
                    }


                    List<int> ous_selected = new List<int>();
                    foreach (var item in listaOusSelected)
                    {
                        if (item >= 0)
                        {
                            ous_selected.Add(item);
                        }
                    }

                    
                    var mat_hold = _MexicanaHoldsContext.MaterialCts.Where(mat => mat.CtsRetursId == Cts.CtsReturns.Itemid).ToList();
                    string num_part = "", etaString = "", dm = "";
                    DateTime? eta = null;

                    if (mat_hold.Count > 0)
                    {
                        eta = mat_hold.Where(mat => mat.Eta.HasValue).Select(mat => mat.Eta.Value).LastOrDefault();

                        dm = String.Join(", ", mat_hold.Select(mat => mat.Dm).ToList());
                        num_part = String.Join(", ", mat_hold.Select(mat => mat.PartNumber).ToList());
                    }

                    if (eta != null)
                    {
                        etaString = eta.Value.ToString("yyyy-MM-dd");
                    }


                    if (ous_selected.Count == 0)
                    {
                        CtsRetReturn.CtsReturns.Itemid = CtstoUpdate.Itemid;
                        CtsRetReturn.CtsReturns.Customer = CtstoUpdate.Customer;
                        //CtsRetReturn.CtsReturns.day = Convert.ToInt32((today - updatedHold.start_hold).Value.Days);
                        //CtsRetReturn.CtsReturns.total_days_hold = Convert.ToInt32((today - updatedHold.start_chasis_date.Value).Days);
                        CtsRetReturn.CtsReturns.Discrepancy = CtstoUpdate.Discrepancy;
                        CtsRetReturn.CtsReturns.Material = CtstoUpdate.Material;
                        //CtsRetReturn.CtsReturns.o = "NA";
                        //CtsRetReturn.CtsReturns.ou_resp_id = "0";
                        CtsRetReturn.CtsReturns.ReleaseMfgDate = CtstoUpdate.ReleaseMfgDate;
                        CtsRetReturn.CtsReturns.StartChasisDate = CtstoUpdate.StartChasisDate;
                        CtsRetReturn.CtsReturns.StartCts = CtstoUpdate.StartCts;
                        CtsRetReturn.CtsReturns.Vin = CtstoUpdate.Vin;
                        CtsRetReturn.CtsReturns.UserRequestedCts = CtstoUpdate.UserRequestedCts;
                        CtsRetReturn.CtsReturns.UserAcceptIngress = CtstoUpdate.UserAcceptIngress;
                        CtsRetReturn.CtsReturns.Material = CtstoUpdate.Material;
                        CtsRetReturn.CtsReturns.EtaFinishCtsQa = CtstoUpdate.EtaFinishCtsQa;
                        CtsRetReturn.CtsReturns.EtaCommentCtsRepair = CtstoUpdate.EtaCommentCtsRepair;
                        CtsRetReturn.CtsReturns.CommentRepair = CtstoUpdate.CommentRepair;
                        _MexicanaHoldsContext.SaveChanges();
                    }
                    else
                    {
                   
                        int result_cts2 = 1;
                        int idcts = CtstoUpdate.Itemid;
                        int count2 = 0;

                        foreach (var item in ous_selected)
                        {
                            if (!OusSinModificar.Contains(item))
                            {
                                CtsOu new_cts_ou = new CtsOu
                                {
                                    CtsRetursId = idcts,
                                    OuId = item,
                                    Active = true,
                                    CtsOuRepairComment = "sin comentario reparacion",
                                    CtsOuRepair = CtstoUpdate.StartCts!=null? CtstoUpdate.StartCts: DateTime.Now.AddHours(48),
                                    CtsOuRepairEditBy = CtstoUpdate.UserEdit

                                };

                                _MexicanaHoldsContext.CtsOu.Add(new_cts_ou);
                                count2++;
                            }

                        }

                        if (count2 > 0)
                        {
                            result_cts2 = _MexicanaHoldsContext.SaveChanges();
                        }
                        if (result_cts2 == 0)
                        {
                            return Json(new
                            {
                                result = false,
                                message = "Error saving OUs reponsible, only cts updated."
                            });

                        }
                        else
                        {

                          
                            var ous =  _MexicanaHoldsContext.Ou.Where(x => ous_selected.Contains(x.Itemid)).ToList();

                            CtsRetReturn.CtsReturns.Itemid = CtstoUpdate.Itemid;
                            CtsRetReturn.CtsReturns.Customer = CtstoUpdate.Customer;
                            CtsRetReturn.CtsReturns.Discrepancy = CtstoUpdate.Discrepancy;
                            CtsRetReturn.CtsReturns.Material = CtstoUpdate.Material;
                            CtsRetReturn.CtsReturns.ReleaseMfgDate = CtstoUpdate.ReleaseMfgDate;
                            CtsRetReturn.CtsReturns.StartChasisDate = CtstoUpdate.StartChasisDate;
                            CtsRetReturn.CtsReturns.StartCts = CtstoUpdate.StartCts;
                            CtsRetReturn.CtsReturns.Vin = CtstoUpdate.Vin;
                            CtsRetReturn.CtsReturns.UserRequestedCts = CtstoUpdate.UserRequestedCts;
                            CtsRetReturn.CtsReturns.UserAcceptIngress = CtstoUpdate.UserAcceptIngress;
                            CtsRetReturn.CtsReturns.UserEdit = CtstoUpdate.UserEdit;
                            CtsRetReturn.CtsReturns.EtaCommentCts = CtstoUpdate.EtaCommentCts;
                            CtsRetReturn.CtsReturns.EtaFinishCtsQa = CtstoUpdate.EtaFinishCtsQa;
                            CtsRetReturn.CtsReturns.EtaCommentCtsRepair = CtstoUpdate.EtaCommentCtsRepair;
                            CtsRetReturn.CtsReturns.CommentRepair = CtstoUpdate.CommentRepair;
                            //    hold_return.eta_comment_hold_repair = holdPosted.eta_comment_hold_repair;
                            _MexicanaHoldsContext.SaveChanges();
                        }
                    }
                    _MexicanaHoldsContext.LogCtsReturns.Add(new LogCtsReturns
                    {
                        Date = DateTime.Now,
                        TableLog = "cts item id" + CtsRetReturn.CtsReturns.Itemid,
                        TextLog = "id " + CtsRetReturn.CtsReturns.Itemid + " user: " + CtsRetReturn.CtsReturns.UserEdit + " old OU: " + string.Join(", ", OusSinModificar.Select(x => x)) + " New OU is: " + _MexicanaHoldsContext.Ou.Where(x => CtsRetReturn.CtsOu.Select(x=>x.Itemid).Contains(x.Itemid)).Select(x=>x.OuName),
                        UserLog = Cts.CtsReturns.UserEdit
                    });

                _MexicanaHoldsContext.UnitHistoryCts.Add(new UnitHistoryCts
                    {
                        OuActual = string.Join(", ", OusSinModificar.Select(x => x)),
                        UserHistory = Cts.CtsReturns.UserEdit,
                        //PosixTimestamp = 1,//modificar tipo de dato en BD
                        Vin = Cts.CtsReturns.Vin.ToUpper(),
                        Notes = " id " + Cts.CtsReturns.Itemid + " user: " + Cts.CtsReturns.UserEdit + " old OU: " + string.Join(", ", OusSinModificar.Select(x => x)) + " New OU is: " + _MexicanaHoldsContext.Ou.Where(x => CtsRetReturn.CtsOu.Select(x => x.Itemid).Contains(x.Itemid)).Select(x => x.OuName),
                        Itemid = Cts.CtsReturns.Itemid
                    });
                    int save_history = _MexicanaHoldsContext.SaveChanges();

                    if (save_history>1) { Console.WriteLine("Si guardó historial"); }
                    else { Console.WriteLine("No guardó historial"); }
                }
                else
                {
                    return Json(new
                    {
                        result = false,
                        message_2 = "Desde le que no truena",
                        message = "Error Update cts "
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //var mail = new EMail();
                //mail.SendMailError(ex.Message, ex);
                return Json(new
                {
                    message_2 = "Desde le que si truena " + ex.Message,
                    result = false,
                    message = "Error Update Hold"
                });
            }



            return Json(new
            {
                result = true,
                message = "saving OUs reponsible, Succefully"
            });

        }


    }
}
