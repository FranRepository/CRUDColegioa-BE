using ColegioCRUDApp.DataAccess;
using ColegioCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using ColegioCRUDApp.Interfaces;
using ColegioCRUDApp.UnitOfWork;

namespace ColegioCRUDApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AlumnoGradoController : ControllerBase
    {
        private readonly ColegioCRUDContext _colegioCRUDContext;

        public AlumnoGradoController(ColegioCRUDContext colegioCRUDContext)
        {
            _colegioCRUDContext = colegioCRUDContext;
        }

        [HttpGet("{id}", Name = "GetAlumnoGrado")]
        public IActionResult Get(int id)
        {
            try
            {
                AlumnoGrado alumnoGrado = _colegioCRUDContext.AlumnoGrado.Find(id);

                if (alumnoGrado == null)
                {
                    return NotFound("No AlumnoGrado found");
                }

                AlumnoGrado1 alumnoGrado1 = new AlumnoGrado1
                {
                 
                    Id = alumnoGrado.Id,
                    AlumnoId = alumnoGrado.Id,
                    GradoId= alumnoGrado.GradoId,
                    Seccion = alumnoGrado.Seccion,
                };

                return Ok(alumnoGrado1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "InsertAlumnoGrado")]
        public IActionResult Insert(AlumnoGrado1 alumnoGrado)
        {
            try
            {
                AlumnoGrado newAlumnoGrado = new AlumnoGrado
                {
                    AlumnoId = alumnoGrado.Id,
                    GradoId = alumnoGrado.GradoId,
                    Seccion = alumnoGrado.Seccion,
                };

                _colegioCRUDContext.AlumnoGrado.Add(newAlumnoGrado);
                int qty = _colegioCRUDContext.SaveChanges();

                if (qty > 0)
                {
                    return Ok();
                }

                return NotFound("No AlumnoGrado Insert");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "UpdateAlumnoGrado")]
        public IActionResult Update(AlumnoGrado1 alumnoGrado)
        {
            try
            {
                AlumnoGrado existingAlumnoGrado = _colegioCRUDContext.AlumnoGrado.Find(alumnoGrado.Id);

                if (existingAlumnoGrado == null)
                {
                    return NotFound("AlumnoGrado not found");
                }

                
                existingAlumnoGrado.Id = alumnoGrado.Id;
                existingAlumnoGrado.AlumnoId = alumnoGrado.AlumnoId;
                existingAlumnoGrado.GradoId = alumnoGrado.GradoId;
                existingAlumnoGrado.Seccion = alumnoGrado.Seccion;


                _colegioCRUDContext.Entry(existingAlumnoGrado).CurrentValues.SetValues(alumnoGrado);
                _colegioCRUDContext.SaveChanges();

          

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}", Name = "DeleteAlumnoGrado")]
        public IActionResult Delete(int id)
        {
            try
            {
                AlumnoGrado existingAlumnoGrado = _colegioCRUDContext.AlumnoGrado.Find(id);

                if (existingAlumnoGrado == null)
                {
                    return NotFound("AlumnoGrado not found");
                }

                _colegioCRUDContext.AlumnoGrado.Remove(existingAlumnoGrado);
                 var delete=  _colegioCRUDContext.SaveChanges();

                if (delete>=1)
                {

                    return Ok();  
                }
                else { return NotFound("No AlumnoGrado Delete");}

              
            }
            catch (Exception ex)
            {
           
                return StatusCode(500, "Internal server error");
            }
        }

        // AlumnoGradoController.cs

        [HttpGet(Name = "GetAllAlumnosGrado")]
        public IActionResult GetAll()
        {
            try
            {
                List<AlumnoGrado> alumnosGrado = _colegioCRUDContext.AlumnoGrado.ToList();

                if (alumnosGrado == null || alumnosGrado.Count == 0)
                {
                    return NotFound("No AlumnosGrado found");
                }

                List<AlumnoGrado1> alumnosGrado1 = new List<AlumnoGrado1>();

                foreach (var alumnoGrado in alumnosGrado)
                {
                    alumnosGrado1.Add(new AlumnoGrado1
                    {
                        Id = alumnoGrado.Id,
                        AlumnoId = alumnoGrado.AlumnoId,
                        GradoId = alumnoGrado.GradoId,
                        Seccion = alumnoGrado.Seccion
                    });
                }

                return Ok(alumnosGrado1);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
