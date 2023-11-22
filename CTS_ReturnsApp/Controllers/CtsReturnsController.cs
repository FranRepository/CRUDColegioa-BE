using CTS_ReturnsApp.Models;
using CTS_ReturnsApp.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        public CtsReturnsController(IUnitOfWork unitOfWork, IUnitOfWorkDb2 unitOfWorkDb2, MexicanaHoldsContext mexicanaHoldsContext)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkDb2 = unitOfWorkDb2;
            _MexicanaHoldsContext = mexicanaHoldsContext;
        }
        [HttpGet(Name = "UnitData")]
        public InfoUnitResult UnitData(string VIN)
        {

            try
            {

                JsonResult JsonR = _unitOfWorkDb2.Db2.InfoTruck(VIN);
                string JsonStr = JsonConvert.SerializeObject(JsonR.Value);
                InfoUnitResult responseUnitInfo = JsonConvert.DeserializeObject<InfoUnitResult>(JsonStr);


                return responseUnitInfo;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost(Name = "NewCts")]
        public async Task<JsonResult> NewCts(CtsAndOus CtsAndOus)
        {

            if (CtsAndOus.CtsReturns.Itemid != 0)
            {
                return await UpdateHold(CtsAndOus);
            }
            else
            {
                if (CtsAndOus.CtsReturns.Vin != "" && CtsAndOus.CtsReturns.Itemid == 0)
                {


                    JsonResult JsonR = _unitOfWorkDb2.Db2.InfoTruck(CtsAndOus.CtsReturns.Vin);
                    string JsonStr = JsonConvert.SerializeObject(JsonR.Value);
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

                    if (CtsAndOus.UserData != null)
                        CtsAndOus.CtsReturns.FinderBy = _MexicanaHoldsContext.Ou.Where(x => x.OuName.Trim() == CtsAndOus.UserData.Custom.Trim()).Select(x => x.Itemid).FirstOrDefault();
                    else
                        CtsAndOus.CtsReturns.FinderBy = 750;


                    CtsReturns new_cts = CtsAndOus.CtsReturns;
                    _MexicanaHoldsContext.CtsReturns.Add(new_cts);
                    int result_cts = await _MexicanaHoldsContext.SaveChangesAsync();

                    DateTime today = DateTime.Now;


                    if (result_cts > 0)
                    {
                        if (CtsAndOus.CtsOu != null)
                        {
                            foreach (CtsOu ou in CtsAndOus.CtsOu)
                            {
                                //Se agregan las ous

                                CtsOu new_OuCts = ou;
                                new_OuCts.CtsRetursId = _MexicanaHoldsContext.CtsReturns.OrderByDescending(x => x.Itemid).Select(x => x.Itemid).First();
                                _MexicanaHoldsContext.CtsOu.Add(new_OuCts);

                                int resultOus = await _MexicanaHoldsContext.SaveChangesAsync();

                            }
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

                    EMail em = new EMail();
                    var mails = _MexicanaHoldsContext.Mails.Where(ma => ma.Title == "CTS Retorno Saltillo Plant").FirstOrDefault();
                    if (mails != null && mails.Mails1 !="")
                    {
                        mails.Mails1 = mails + ",miguel_angel.rodriguez@daimlertruck.com";
                    }
                    else
                    {
                        mails = new Mails();
                        mails.Mails1 = "miguel_angel.rodriguez@daimlertruck.com;jesus_francisco.gonzalez@daimlertruck.com";
                    }

                    string bodyMail = "TEST: Favor de poner validar la siguiente unidad en Retorno CTS: " + CtsAndOus.CtsReturns.Vin + "</br></br>";
                    bodyMail += "Por el concepto de: " + CtsAndOus.CtsReturns.ItemAndCode + "</br>";
                    bodyMail += "Y la discrepancia: " + CtsAndOus.CtsReturns.Discrepancy + "</br>";

                    if (CtsAndOus.UserData != null)
                        em.SendMail(bodyMail, mails.Mails1, "Retornos CTS - Solicitud de ingreso a Retorno CTS: " + CtsAndOus.CtsReturns.Vin, CtsAndOus.UserData.Email);
                    else
                        em.SendMail(bodyMail, mails.Mails1, "Retornos CTS - Solicitud de ingreso a Retorno CTS: " + CtsAndOus.CtsReturns.Vin, "");


                    _MexicanaHoldsContext.LogCtsReturns.Add(new LogCtsReturns
                    {
                        Date = DateTime.Now,
                        TableLog = "Solicitud ingreso Cts",
                        TextLog = " vin " + CtsAndOus.CtsReturns.Vin,
                        UserLog = CtsAndOus.CtsReturns.UserRequestedCts
                    });
                    int save_history1 = await _MexicanaHoldsContext.SaveChangesAsync();

                    var OusSelected = "";
                    List<int?> OusSelectedID = new List<int?>();

                    if (CtsAndOus.CtsOu != null)
                    {
                        OusSelectedID = CtsAndOus.CtsOu.Select(x => x.OuId).ToList();
                        OusSelected = string.Join(", ", _MexicanaHoldsContext.Ou.Where(x => OusSelectedID.Contains(x.Itemid)).Select(x => x.OuName).ToList());
                    }

                    _MexicanaHoldsContext.UnitHistoryCts.Add(new UnitHistoryCts
                    {
                        OuActual = OusSelected,
                        UserHistory = CtsAndOus.CtsReturns.UserRequestedCts,
                        PosixTimestamp = Convert.ToInt32(DateTime.Now.ToUniversalTime()),
                        Vin = CtsAndOus.CtsReturns.Vin.ToUpper(),
                        Notes = CtsAndOus.CtsReturns.Discrepancy,
                        CtsRetursId = CtsAndOus.CtsReturns.Itemid
                    });
                    int save_history = await _MexicanaHoldsContext.SaveChangesAsync();




                    if (save_history >= 1 && save_history1 >= 1) { Console.WriteLine("Si guardó historial"); }
                    else { Console.WriteLine("No guardó historial"); }

                    return Json(new
                    {
                        result = true,
                        message = "Save succeful new CTS Return",

                    });
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
            }
        }


        [HttpPost(Name = "AcceptRequest")]
        public async Task<JsonResult> AcceptOrDeclineRequest(CtsAndOuAcceptOrDeclineRequest CtsAndOuAcceptOrDeclineRequest)
        {
            try
            {
                        CtsAndOus ctsAndOus = new CtsAndOus()
                    {
                        CtsReturns = _MexicanaHoldsContext.CtsReturns.Where(x => x.Itemid == CtsAndOuAcceptOrDeclineRequest.ItemId).FirstOrDefault(),
                        UserData = CtsAndOuAcceptOrDeclineRequest.UserData,
                        CtsOu = CtsAndOuAcceptOrDeclineRequest.CtsOu

                    };

                    if (ctsAndOus.CtsReturns!=null && ctsAndOus.CtsReturns.Itemid != 0)
                    {
                        return await UpdateHold(ctsAndOus);
                    }
                    else
                    {
                        return Json(new
                        {
                            result = false,
                            message = "CTS Itemid is Null",
                            exception = "CTS Itemid is Null"
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

        }

        [HttpPost(Name = "UpdateHold")]
        public async Task<JsonResult> UpdateHold(CtsAndOus Cts)
        {
            try
            {
                var CtstoUpdate = _MexicanaHoldsContext.CtsReturns.FirstOrDefault(x => x.Itemid == Cts.CtsReturns.Itemid);
                var CtsRetReturn = new CtsAndOus();


                CtstoUpdate.FinderBy = _MexicanaHoldsContext.Ou.Where(x => x.OuName.Trim() == Cts.UserData.Custom.Trim()).Select(x => x.Itemid).FirstOrDefault();

                CtstoUpdate.Itemid = Cts.CtsReturns.Itemid;
                CtstoUpdate.Discrepancy = Cts.CtsReturns.Discrepancy;
                CtstoUpdate.Material = Convert.ToBoolean(Cts.CtsReturns.Material);
                CtstoUpdate.CommentRepair = Cts.CtsReturns.CommentRepair;
                CtstoUpdate.EtaCommentCtsRepair = Convert.ToDateTime(Cts.CtsReturns.EtaCommentCtsRepair);
                CtstoUpdate.UserEdit = Cts.CtsReturns.UserEdit;
                DateTime today = DateTime.Now;


                var material_hold = _MexicanaHoldsContext.MaterialCts.Where(mat => mat.CtsRetursId == Cts.CtsReturns.Itemid).ToList();
                var OusSinModificar = _MexicanaHoldsContext.CtsOu.Where(x => x.CtsRetursId == Cts.CtsReturns.Itemid).OrderBy(x => x.Itemid).Select(x => x.OuId).ToList();
                var idsCtsOus = _MexicanaHoldsContext.CtsOu.Where(x => x.CtsRetursId == Cts.CtsReturns.Itemid).Select(x => x).ToList();
                int result_hold = 1;

                int[]? listaOusSelected = null;

                if (Cts.CtsOu != null)
                {
                    listaOusSelected = Cts.CtsOu
                       .Select(x => x.Itemid)
                       .ToArray();
                }


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
                        result_hold = await _MexicanaHoldsContext.SaveChangesAsync();
                    }
                }



                if (result_hold > 0)
                {
                    if (Cts.CtsOu != null)
                    {
                        var OusSelected = Cts.CtsOu.OrderBy(x => x.Itemid);

                        int i = 0;
                        foreach (CtsOu ou in OusSelected)
                        {

                            if (ou.OuId != 0)
                            {
                                listaOusSelected[i] = (int)ou.OuId;
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
                                if (ReleaseCount == CtsOu.Count)
                                {
                                    isDifferent = true;
                                    foreach (CtsOu cts_ou in CtsOu)
                                    {
                                        if (cts_ou.Active == false)
                                        {
                                            cts_ou.Active = true;
                                            await _MexicanaHoldsContext.SaveChangesAsync();
                                        }
                                    }

                                    TextAdittionarEmail = "</br><b> En caso de haber liberado anteriormente favor de confirmar liberación:</br> liberando " +
                                        "nuevamente por Reasignación</b></br> ";
                                }
                            }

                            if (isDifferent)
                            {

                                EMail em = new EMail();
                                var mails = _MexicanaHoldsContext.Mails.Where(ma => ma.Title == "Released").FirstOrDefault();

                                var ous1 = _MexicanaHoldsContext.Ou.Where(ous => OusSinModificar.Contains(ous.Itemid)).Select(x => x.OuName).ToList();
                                var ous2 = _MexicanaHoldsContext.Ou.Where(ous => listaOusSelected.Contains(ous.Itemid)).Select(x => x.OuName).ToList();

                                string OUsMails = string.Join(",", _MexicanaHoldsContext.Ou.Where(x => listaOusSelected.Contains(x.Itemid)).Select(x => x.MailTo.ToString()));

                                mails.Mails1 += ",miguel_angel.rodriguez@daimler.com";
                                mails.Mails1 += "," + OUsMails;

                                string bodyMail = "";

                                if(CtstoUpdate.StatusCts!=-1)
                                    bodyMail=   "Se reasigno unidad en CTS Returns, Unidad: " + CtstoUpdate.Vin + "</br></br>";
                                else
                                    bodyMail = "Se asigno unidad en CTS Returns, Unidad: " + CtstoUpdate.Vin + "</br></br>";


                                bodyMail += "Y la discrepancia: " + CtstoUpdate.Discrepancy + "</br>";

                                if (CtstoUpdate.StatusCts != -1)
                                    bodyMail += "OUs anteriores: " + string.Join(",", ous1) + "</br>";

                                bodyMail += "OUs a quien se asigno: " + string.Join(",", ous2) + " </br>";

                                bodyMail += TextAdittionarEmail;
                                //em.SendMail(bodyMail, mails.Mails1, "Holds - CTS Returns Request: " + CtstoUpdate.Vin, /*CtstoUpdate.mail_user*/);

                            }
                        }
                    }


                    List<int> ous_selected = new List<int>();
                    if (listaOusSelected != null)
                    {
                        foreach (var item in listaOusSelected)
                        {
                            if (item >= 0)
                            {
                                ous_selected.Add(item);
                            }
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
                        CtsRetReturn.CtsReturns = CtstoUpdate;

                        await _MexicanaHoldsContext.SaveChangesAsync();
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
                                    CtsOuRepair = CtstoUpdate.StartCts != null ? CtstoUpdate.StartCts : DateTime.Now.AddHours(48),
                                    CtsOuRepairEditBy = CtstoUpdate.UserEdit

                                };

                                _MexicanaHoldsContext.CtsOu.Add(new_cts_ou);
                                count2++;
                            }

                        }

                        if (count2 > 0)
                        {
                            result_cts2 = await _MexicanaHoldsContext.SaveChangesAsync();
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


                            var ous = _MexicanaHoldsContext.Ou.Where(x => ous_selected.Contains(x.Itemid)).ToList();

                            CtsRetReturn.CtsReturns = CtstoUpdate;

                            //    hold_return.eta_comment_hold_repair = holdPosted.eta_comment_hold_repair;
                            await _MexicanaHoldsContext.SaveChangesAsync();
                        }
                    }
                    string modificacionOU = "";
                    if (CtsRetReturn.CtsOu != null)
                    {
                        if (OusSinModificar != null)
                            modificacionOU = " old OU: " + string.Join(", ", OusSinModificar.Select(x => x)) + ", New OU is: " + String.Join(", ", _MexicanaHoldsContext.Ou.Where(x => CtsRetReturn.CtsOu.Select(x => x.Itemid).Contains(x.Itemid)).Select(x => x.OuName));
                        else
                            modificacionOU = " old OU: SIN OU, New OU is: " + String.Join(", ", _MexicanaHoldsContext.Ou.Where(x => CtsRetReturn.CtsOu.Select(x => x.Itemid).Contains(x.Itemid)).Select(x => x.OuName));

                    }

                    _MexicanaHoldsContext.LogCtsReturns.Add(new LogCtsReturns
                    {
                        Date = DateTime.Now,
                        TableLog = "cts item id" + CtsRetReturn.CtsReturns.Itemid,

                        TextLog = "id " + CtsRetReturn.CtsReturns.Itemid + " user: " + CtsRetReturn.CtsReturns.UserEdit + modificacionOU,
                        UserLog = Cts.CtsReturns.UserEdit
                    });


                    _MexicanaHoldsContext.UnitHistoryCts.Add(new UnitHistoryCts
                    {
                        OuActual = string.Join(", ", OusSinModificar.Select(x => x)),
                        UserHistory = Cts.CtsReturns.UserEdit,
                        PosixTimestamp = Convert.ToInt32(DateTime.Now.ToUniversalTime()),
                        Vin = Cts.CtsReturns.Vin.ToUpper(),
                        Notes = " id " + Cts.CtsReturns.Itemid + " user: " + Cts.CtsReturns.UserEdit + modificacionOU,
                        CtsRetursId = Cts.CtsReturns.Itemid
                    });
                    int save_history = await _MexicanaHoldsContext.SaveChangesAsync();

                    if (save_history > 1) { Console.WriteLine("Si guardó historial"); }
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
        [HttpGet(Name = "GetCtsBucketList")]
        public List<CtsReturnSqlReturn> GetCtsBucketList(/*DateTime STARTDATE, DateTime ENDDATE,*/int OPTION, int OU, int role)
        {
            //STARTDATE = DateTime.Parse(STARTDATE.ToShortDateString() + " 00:00:00");
            //ENDDATE = DateTime.Parse(ENDDATE.ToShortDateString() + " 23:59:59");
            //Esta es la variable que vamos a regresar en forma de JSON
            List<CtsReturnSqlReturn> result = new List<CtsReturnSqlReturn>();
            int statusToSearch = 0;
            try
            {
                //Para obtener el ou
                var ou = _MexicanaHoldsContext.Ou.Where(x => x.OuNumber == OU).Select(x => new { x.OuName, x.Itemid }).ToList();
                var ouItemsId = _MexicanaHoldsContext.Ou.Where(x => x.OuNumber == OU).Select(x => x.Itemid).ToList();

                List<CtsOu> CtsOuActive;
                List<CtsOu> CtsOuADelibery;
                List<CtsReturns> listCtsDB = new List<CtsReturns>();
                List<CtsOu> CtsOuActiveAndDelibery;

                switch (OPTION)
                {
                    case -1:
                        statusToSearch = -1;   //Request
                        break;
                    case 0:
                        //Accepted
                        statusToSearch = 0;
                        break;
                    case 1:
                        //Release by OUs
                        statusToSearch = 1;
                        break;
                    case 2:
                        //Release complete
                        statusToSearch = 2;
                        break;
                    case 3:
                        //Release complete
                        statusToSearch = 3;
                        break;
                    default:
                        // code block
                        break;
                }


                if (role == 1)
                {
                    listCtsDB = _MexicanaHoldsContext.CtsReturns.Where(x => x.StatusCts == statusToSearch && x.Active == true).ToList();
                    CtsOuActive = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == true).ToList();
                    CtsOuADelibery = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == false).ToList();

                    CtsOuActiveAndDelibery = new List<CtsOu>();
                    CtsOuActiveAndDelibery.AddRange(CtsOuActive);
                    CtsOuActiveAndDelibery.AddRange(CtsOuADelibery);
                }
                else
                {
                    if (statusToSearch != -1)
                    {
                        CtsOuActive = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == true && ouItemsId.Contains((int)x.OuId)).ToList();
                        CtsOuADelibery = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == false && ouItemsId.Contains((int)x.OuId)).ToList();

                        CtsOuActiveAndDelibery = new List<CtsOu>();
                        CtsOuActiveAndDelibery.AddRange(CtsOuActive);
                        CtsOuActiveAndDelibery.AddRange(CtsOuADelibery);

                        List<int> ctsIds = CtsOuActiveAndDelibery.Select(x => (int)x.CtsRetursId).ToList();

                        listCtsDB = _MexicanaHoldsContext.CtsReturns.Where(x => x.StatusCts == statusToSearch && x.Active == true && ctsIds.Contains(x.Itemid)).ToList();
                    }
                    else
                    {
                        listCtsDB = _MexicanaHoldsContext.CtsReturns.Where(x => x.StatusCts == statusToSearch && x.Active == true).ToList();
                        CtsOuActive = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == true).ToList();
                        CtsOuADelibery = _MexicanaHoldsContext.CtsOu.Where(x => x.Active == false).ToList();

                        CtsOuActiveAndDelibery = new List<CtsOu>();
                        CtsOuActiveAndDelibery.AddRange(CtsOuActive);
                        CtsOuActiveAndDelibery.AddRange(CtsOuADelibery);
                    }
                }

                List<CtsAndOus> ctsAndOusReturn = new(
                    );

                foreach (CtsReturns cts in listCtsDB)
                {
                    CtsAndOus ctsAndOus = new CtsAndOus()
                    {
                        CtsReturns = cts,
                        CtsOu = CtsOuActiveAndDelibery.Where(x => x.CtsRetursId == cts.Itemid).ToList()
                    };


                    ctsAndOusReturn.Add(ctsAndOus);
                }


                CtsReturnSqlReturn ctsReturnCtsSql;
                foreach (CtsAndOus CtsAndOusIdentity in ctsAndOusReturn)
                {
                    var ou_resp_id = (CtsAndOusIdentity != null && CtsAndOusIdentity.CtsOu != null) ? CtsAndOusIdentity.CtsOu.Where(x => x.Active == true).Select(x => (int)x.OuId).ToList() : new List<int>() { 0 };
                    var ous_resp_all = (CtsAndOusIdentity != null & CtsAndOusIdentity.CtsOu != null) ? CtsAndOusIdentity.CtsOu.Select(x => (int)x.OuId).ToList() : new List<int>() { 0 };
                    var oufinderid = (CtsAndOusIdentity.CtsReturns != null && CtsAndOusIdentity.CtsReturns.FinderBy != null) ? CtsAndOusIdentity.CtsReturns.FinderBy : null;
                    var ou_finder = _MexicanaHoldsContext.Ou.Where(x => x.Itemid == oufinderid).Select(x => x.OuName).FirstOrDefault();
                    ctsReturnCtsSql = new CtsReturnSqlReturn
                    {
                        itemid = CtsAndOusIdentity.CtsReturns.Itemid,
                        vin = CtsAndOusIdentity.CtsReturns.Vin,
                        customer = CtsAndOusIdentity.CtsReturns.Customer,
                        days_cts = 0,
                        total_days_cts = 0,
                        discrepancy = CtsAndOusIdentity.CtsReturns.Discrepancy,
                        material = CtsAndOusIdentity.CtsReturns.Material != null ? (bool)CtsAndOusIdentity.CtsReturns.Material : false,
                        ou_resp_id = ou_resp_id,
                        ous_resp_all = ous_resp_all,
                        ou_res_name_all = String.Join(", ", _MexicanaHoldsContext.Ou.Where(x => ou_resp_id.Contains(x.Itemid)).Select(x => x.OuName).ToList()),
                        ou_res_name_pdtes = String.Join(", ", _MexicanaHoldsContext.Ou.Where(x => ou_resp_id.Contains(x.Itemid) && x.Active == true).Select(x => x.OuName).ToList()),
                        release_mfg_date = CtsAndOusIdentity.CtsReturns.ReleaseMfgDate == null ? "" : CtsAndOusIdentity.CtsReturns.ReleaseMfgDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                        start_chasis_date = CtsAndOusIdentity.CtsReturns.StartChasisDate != null ? CtsAndOusIdentity.CtsReturns.StartChasisDate?.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        start_cts = CtsAndOusIdentity.CtsReturns.StartCts != null ? CtsAndOusIdentity.CtsReturns.StartCts?.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        requested_cts = CtsAndOusIdentity.CtsReturns.RequestedCts.ToString("yyyy-MM-dd HH:mm:ss"),
                        user = CtsAndOusIdentity.CtsReturns.UserRequestedCts,
                        active = CtsAndOusIdentity.CtsReturns.Active != null ? (bool)CtsAndOusIdentity.CtsReturns.Active : false,
                        status = CtsAndOusIdentity.CtsReturns.StatusCts,
                        materials_req = "",
                        eta_comment_cts = CtsAndOusIdentity.CtsReturns.EtaCommentCts == null ? "" : CtsAndOusIdentity.CtsReturns.EtaCommentCts,
                        eta_comment_cts_repair = CtsAndOusIdentity.CtsReturns.EtaCommentCtsRepair != null ? CtsAndOusIdentity.CtsReturns.EtaCommentCtsRepair?.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        comment_repair = CtsAndOusIdentity.CtsReturns.CommentRepair,
                        item_and_code = CtsAndOusIdentity.CtsReturns.ItemAndCode,
                        ou_finder = ou_finder != null ? ou_finder : "",
                        users_repair_ou = CtsAndOusIdentity.CtsOu != null ? string.Join(", ", CtsAndOusIdentity.CtsOu.Select(x => x.CtsOuRepairEditBy)) : "",
                        user_accept_repair = CtsAndOusIdentity.CtsReturns.UserAcceptRepair,
                        user_accept_ingress = CtsAndOusIdentity.CtsReturns.UserAcceptIngress,
                    };
                    result.Add(ctsReturnCtsSql);
                }


            }
            catch (Exception ex)
            {
                //var em = new EMail();
                //em.SendMailError(ex.Message, ex);
                throw;
            }
            result = result.OrderByDescending(x => x.days_cts).ToList();

            return result;
        }

        [HttpGet(Name = "GetIssuesShopIssueShoptechLst")]
        public List<IssueSTlModelSimple> GetIssuesShopIssueShoptechLst(string vin)
        {
            DateTime first = DateTime.Today.AddMonths(-2);
            DateTime second = DateTime.Now;
            return _unitOfWorkDb2.Db2.GetIssuesShopIssueShoptechList(first, second, vin);
        }
        [HttpGet(Name = "GetOusLists")]
        public List<OusList> GetOusLists()
        {
            List<OusList> ousList = new List<OusList>();

            // Assuming that Ou has a property named OuNumber and OuName
            List<Ou> uList = _MexicanaHoldsContext.Ou.ToList();

            // Map properties from Ou to OusList
            foreach (var ou in uList)
            {
                OusList ouListItem = new OusList
                {
                    Itemid = ou.Itemid,
                    OuName = ou.OuName,
                    OuNumber = ou.OuNumber
                };

                ousList.Add(ouListItem);
            }

            return ousList;
        }


    }
}
