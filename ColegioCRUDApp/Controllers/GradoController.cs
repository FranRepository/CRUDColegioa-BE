
using ColegioCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ColegioCRUDApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class GradoController : ControllerBase
    {
        private readonly ColegioCRUDContext _colegioCRUDContext;

        public GradoController(ColegioCRUDContext colegioCRUDContext)
        {
            _colegioCRUDContext = colegioCRUDContext;
        }

        [HttpGet(Name = "GetGrado")]
        public IActionResult Get(int id)
        {
            try
            {
                Grado Grado = _colegioCRUDContext.Grado.Find(id);
                Grado1 Grado1 = new Grado1();
                if (Grado != null)
                {
                    Grado1.Nombre = Grado.Nombre;
                    Grado1.ProfesorId = Grado.ProfesorId;
                    Grado1.Id = Grado.Id;

                }
                else
                {
                    return NotFound("No Grado found");
                }

                return Ok(Grado1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "InsertGrado")]
        public IActionResult Insert(Grado1 grado1)
        {
            try
            {
                Grado grado = new Grado
                {
                    Nombre = grado1.Nombre,
                    ProfesorId = grado1.ProfesorId,
                    Id = grado1.Id
                };

                _colegioCRUDContext.Grado.Add(grado);
                int qty = _colegioCRUDContext.SaveChanges();

                if (qty == 0)
                {
                    return BadRequest("Failed to insert Grado");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "UpdateGrado")]
        public IActionResult Update(Grado1 grado)
        {
            try
            {
                Grado existingGrado = _colegioCRUDContext.Grado.Find(grado.Id);

                if (existingGrado == null)
                {
                    return NotFound("Grado not found");
                }

                _colegioCRUDContext.Entry(existingGrado).CurrentValues.SetValues(grado);
                _colegioCRUDContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete(Name = "DeleteGrado")]
        public IActionResult Delete(int id)
        {
            try
            {
                Grado existingGrado = _colegioCRUDContext.Grado.Find(id);

                if (existingGrado == null)
                {
                    return NotFound("Grado not found");
                }

                _colegioCRUDContext.Grado.Remove(existingGrado);
              

                var delete = _colegioCRUDContext.SaveChanges();

                if (delete >= 1)
                {

                    return Ok();
                }
                else { return NotFound("No Grado Delete"); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GradoController.cs

        [HttpGet(Name = "GetAllGrados")]
        public IActionResult GetAll()
        {
            try
            {
                List<Grado> grados = _colegioCRUDContext.Grado.ToList();

                if (grados == null || grados.Count == 0)
                {
                    return NotFound("No Grados found");
                }

                List<Grado1> grados1 = new List<Grado1>();

                foreach (var grado in grados)
                {
                    grados1.Add(new Grado1
                    {
                        Id = grado.Id,
                        Nombre = grado.Nombre,
                        ProfesorId = grado.ProfesorId
                    });
                }

                return Ok(grados1);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
