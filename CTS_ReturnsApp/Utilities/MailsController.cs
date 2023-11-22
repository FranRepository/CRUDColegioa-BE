using CTS_ReturnsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CTS_ReturnsApp.Utilities
{
    [ApiController]
    [Route("api/[controller]/[action]")]

    public class MailsController : Controller
    {
        private readonly MexicanaHoldsContext _mexicanaHolds;

        public MailsController(MexicanaHoldsContext mexicanaHolds)
        {
            _mexicanaHolds = mexicanaHolds ?? throw new ArgumentNullException(nameof(mexicanaHolds));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var listReturn = _mexicanaHolds.Mails.Where(m => m.Enabled == true).ToList();
            return Ok(listReturn);
        }

        [HttpPost]
        public IActionResult NewMails([FromBody] Mails mail)
        {
            try
            {
                _mexicanaHolds.Mails.Add(mail);
                _mexicanaHolds.SaveChanges();
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, message = ex.Message });
            }
            return Ok(mail);
        }

        [HttpPost]
        public IActionResult UpdateMails([FromBody] Mails mail)
        {
            try
            {
                mail.Enabled = true;
                var mailUpdate = _mexicanaHolds.Mails.FirstOrDefault(m => m.Itemid == mail.Itemid);

                if (mailUpdate != null)
                {
                    mailUpdate.Mails1 = mail.Mails1;
                    mailUpdate.Notes = mail.Notes;

                    _mexicanaHolds.SaveChanges();
                }
                else
                {
                    return NotFound(new { result = false, message = "Mail not found" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, message = ex.Message });
            }
            return Ok(mail);
        }

        [HttpPost]
        public IActionResult DeleteMails([FromBody] Mails mail)
        {
            try
            {
                var mailDel = _mexicanaHolds.Mails.FirstOrDefault(m => m.Itemid == mail.Itemid);

                if (mailDel != null)
                {
                    mailDel.Enabled = false;
                    _mexicanaHolds.SaveChanges();
                }
                else
                {
                    return NotFound(new { result = false, message = "Mail not found" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, message = ex.Message });
            }
            return Ok(new { result = true });
        }
    }
}
