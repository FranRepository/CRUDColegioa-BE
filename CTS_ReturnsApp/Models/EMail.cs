using System.Net.Mail;

namespace CTS_ReturnsApp.Models
{
    public class EMail
    {
        public void SendMail(String body, String Emails, String ExtraSobject, string userMail)
        {
            string mailTo;
            if (Emails != "")
            {
                mailTo = Emails;
            }
            else
            {
                mailTo = "jesus_francisco.gonzalez@daimlertruck.com";
            }

            string ProjectName = "Saltillo_TMP_CTS";
            string _sender = ProjectName + "@daimlersoftware.com";
            SmtpClient client = new SmtpClient("wksmtphub.wk.dcx.com");
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            try
            {

                var mail = new MailMessage(_sender.Trim(), mailTo.Trim());
                MailAddress copy2 = new MailAddress("victor_jesus.hernandez@daimlertruck.com");
                MailAddress copy3 = new MailAddress("emilio.gaitan@daimlertruck.com");
                mail.CC.Add(copy2);
                mail.CC.Add(copy3);

                if (userMail != null)
                {
                    if (userMail.Length > 0)
                    {
                        MailAddress copy = new MailAddress(userMail);
                        mail.CC.Add(copy);
                    }
                }

                MailAddress ccopy = new MailAddress("jesus_francisco.gonzalez@daimlertruck.com");



                mail.Bcc.Add(ccopy);
                mail.Subject = ExtraSobject;
                mail.Body += @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional //EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
                    <html xmlns='http://www.w3.org/1999/xhtml' lang='en' xml:lang='en'>
                    <head>
                        <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                        <meta content='width=device-width, initial-scale=1' name='viewport'>
                        <meta content='IE=edge' http-equiv='X-UA-Compatible'>
                        <title>Session marked as done</title>
                        <style>


                            /* CLIENT-SPECIFIC STYLES */
                            body, table, td, a {
                                -webkit-text-size-adjust: 100%;
                                -ms-text-size-adjust: 100%;
                            }

                            table, td {
                                mso-table-lspace: 0pt;
                                mso-table-rspace: 0pt;
                            }

                            img {
                                -ms-interpolation-mode: bicubic;
                            }

                            .hidden {
                                display: none !important;
                                visibility: hidden !important;
                            }

                            /* iOS BLUE LINKS */
                            a[x-apple-data-detectors] {
                                color: inherit !important;
                                text-decoration: none !important;
                                font-size: inherit !important;
                                font-family: inherit !important;
                                font-weight: inherit !important;
                                line-height: inherit !important;
                            }

                            /* ANDROID MARGIN HACK */
                            body {
                                margin: 0 !important;
                            }

                            div[style*='margin: 16px 0'] {
                                margin: 0 !important;
                            }

                            @media only screen and (max-width: 700px) {
                                body, #body {
                                    min-width: 400px !important;
                                }

                                table.wrapper {
                                    width: 100% !important;
                                    min-width: 400px !important;
                                }

                                    table.wrapper &gt; tbody &gt; tr &gt; td {
                                        border-left: 0 !important;
                                        border-right: 0 !important;
                                        border-radius: 0 !important;
                                        padding-left: 10px !important;
                                        padding-right: 10px !important;
                                    }
                            }
                                #DataTable th, #DataTable td {
                                  border: 1px solid black;
                                  border-collapse: collapse;
                                  font-size: 20px;
                                  font-weight: bold;
                                  text-align:center;
                                }
                       #DataTable #theadc{
                                  color:white;
                                  background-color: gray;
                                }

                    #theadc{
                                  color:white;
                                  background-color: gray;
                                }

                       #DataTable #theadcblue{
                                  color:white;
                                background-color: blue;
                                }

                         #DataTable  thead{
                                  background-color: blue;
                                  font-size: 12px;
                                  font-weight: bold;
                                  text-align:center;
                                }
                            body {
                                -webkit-text-size-adjust: 100%;
                                -ms-text-size-adjust: 100%;
                            }

                            img {
                                -ms-interpolation-mode: bicubic;
                            }

                            body {
                                margin: 0 !important;
                            }
                        </style>";

                if (ExtraSobject.Contains("CTS Request"))
                {
                    mail.Body += body + @" <i>Este correo electronico se generó automaticamente. Por favor NO responder a esta dirección de email. </i> </br></br>";
                }
                else
                {
                    mail.Body += body + @" <i>Este correo electronico se genero automaticamente. Por favor NO responder a esta direccion email. </i> </br></br>";
                }
                //body + @" <i>Este correo electrónico se generó automáticamente. Por favor NO responder a este email. </i> </br></br>";
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                EMail e = new EMail();
                e.SendMailError("No se pudo enviar correo", ex);
            }
        }

        public void SendMailError(string msg, Exception ex)
        {
            string ProjectName = "Saltillo_TMP_CTS";
            string _sender = ProjectName + "@daimlersoftware.com";
            SmtpClient client = new SmtpClient("wksmtphub.wk.dcx.com");
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            try
            {

                var mail = new MailMessage(_sender.Trim(), "jesus_francisco.gonzalez@daimlertruck.com");

                mail.Subject = "CTS - La andas regando checale";
                mail.Body = "<i>This email was generated automatically. Please DO NOT reply to this email. </i> </br></br>" +
                            "<b> Project Name:  </b> </br>" + ProjectName +
                             "</br>" +
                            "<b> Message:  </b> </br>" + ex.Message +
                            "</br> <b>InnerException:</b> </br>" + ex.InnerException +
                             "</br>" +
                            "</br> <b>StackTrace:</b>  </br>" + ex.StackTrace +
                            "</br>" +
                            "</br> <b>TargetSite:</b>  </br>" + ex.TargetSite +
                            "</br>" +
                            "</br> <b>Source:</b>  </br>" + ex.Source +
                            "</br>" +
                            "</br> <b>Data:</b> </br>" + ex.Data;



                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception)
            {
                //Email e = new Email();
                //e.SendErrorMail("No se pudeo enviar correo", ex, "");
            }
        }
    }
}
