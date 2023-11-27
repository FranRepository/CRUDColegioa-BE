
using ColegioCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ColegioCRUDApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AlumnoController : ControllerBase
    {
        private readonly ColegioCRUDContext _colegioCRUDContext;

        public AlumnoController(ColegioCRUDContext colegioCRUDContext)
        {
            _colegioCRUDContext = colegioCRUDContext;
        }

        [HttpGet(Name = "GetAlumno")]
        public IActionResult Get(int id)
        {
            try
            {
                Alumno alumno = _colegioCRUDContext.Alumno.Find(id);
                Alumno1 alumno1 = new Alumno1();
                if (alumno != null)
                {
                        alumno1.Nombre = alumno.Nombre;
                        alumno1.Apellidos = alumno.Apellidos;
                        alumno1.Genero = alumno.Genero;
                        alumno1.Id = id;

                }
                else 
                {
                    return NotFound("No Alumno found");
                }

                return Ok(alumno1);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "InsertAlumno")]
        public IActionResult Insert(Alumno1 alumno)
        {
            try
            {
                Alumno alumno1 = new Alumno
                {
                    Nombre = alumno.Nombre,
                    Apellidos = alumno.Apellidos,
                    Genero = alumno.Genero,
                    FechaNacimiento = alumno.FechaNacimiento
                };

                _colegioCRUDContext.Alumno.Add(alumno1);
                int qty = _colegioCRUDContext.SaveChanges();

                if (qty == 0)
                {
                    return BadRequest("Failed to insert Alumno");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost( Name = "UpdateAlumno")]
        public IActionResult Update(Alumno1 alumno)
        {
            try
            {
                Alumno existingAlumno = _colegioCRUDContext.Alumno.Find(alumno.Id);

                if (existingAlumno == null)
                {
                    return NotFound("Alumno not found");
                }

                _colegioCRUDContext.Entry(existingAlumno).CurrentValues.SetValues(alumno);
                _colegioCRUDContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete( Name = "DeleteAlumno")]
        public IActionResult Delete(int id)
        {
            try
            {
                Alumno existingAlumno = _colegioCRUDContext.Alumno.Find(id);

                if (existingAlumno == null)
                {
                    return NotFound("Alumno not found");
                }

                _colegioCRUDContext.Alumno.Remove(existingAlumno);
            

                var delete = _colegioCRUDContext.SaveChanges();

                if (delete >= 1)
                {

                    return Ok();
                }
                else { return NotFound("No Alumno Delete"); }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        // AlumnoController.cs

        [HttpGet(Name = "GetAllAlumnos")]
        public IActionResult GetAll()
        {
            try
            {
                List<Alumno> alumnos = _colegioCRUDContext.Alumno.ToList();

                if (alumnos == null || alumnos.Count == 0)
                {
                    return NotFound("No Alumnos found");
                }

                List<Alumno1> alumnos1 = new List<Alumno1>();

                foreach (var alumno in alumnos)
                {
                    alumnos1.Add(new Alumno1
                    {
                        Id = alumno.Id,
                        Nombre = alumno.Nombre,
                        Apellidos = alumno.Apellidos,
                        Genero = alumno.Genero,
                        FechaNacimiento = alumno.FechaNacimiento
                    });
                }

                return Ok(alumnos1);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
