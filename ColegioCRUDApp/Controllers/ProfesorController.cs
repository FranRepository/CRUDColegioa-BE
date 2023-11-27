
using ColegioCRUDApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ColegioCRUDApp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProfesorController : ControllerBase
    {
        private readonly ColegioCRUDContext _colegioCRUDContext;

        public ProfesorController(ColegioCRUDContext colegioCRUDContext)
        {
            _colegioCRUDContext = colegioCRUDContext;
        }

        [HttpGet("{id}", Name = "GetProfesor")]
        public IActionResult Get(int id)
        {
            try
            {
                Profesor profesor = _colegioCRUDContext.Profesor.Find(id);

                if (profesor == null)
                {
                    return NotFound("Profesor not found");
                }

                Profesor1 profesor1 = new Profesor1
                {
                    Nombre = profesor.Nombre,
                    Apellido = profesor.Apellido,
                    Genero = profesor.Genero,
                    Id = id
                };

                return Ok(profesor1);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "InsertProfesor")]
        public IActionResult Insert(Profesor1 profesor)
        {
            try
            {
                Profesor newProfesor = new Profesor
                {
                    Nombre = profesor.Nombre,
                    Apellido = profesor.Apellido,
                    Genero = profesor.Genero,
                    Id = profesor.Id
                };

                _colegioCRUDContext.Profesor.Add(newProfesor);
                int qty = _colegioCRUDContext.SaveChanges();

                if (qty > 0)
                {
                    return Ok();
                }

                return NotFound("No Profesor Insert");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost(Name = "UpdateProfesor")]
        public IActionResult Update(Profesor1 profesor)
        {
            try
            {
                Profesor existingProfesor = _colegioCRUDContext.Profesor.Find(profesor.Id);

                if (existingProfesor == null)
                {
                    return NotFound("Profesor not found");
                }

                existingProfesor.Nombre = profesor.Nombre;
                existingProfesor.Apellido = profesor.Apellido;
                existingProfesor.Genero = profesor.Genero;
                existingProfesor.Id = profesor.Id;

                _colegioCRUDContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}", Name = "DeleteProfesor")]
        public IActionResult Delete(int id)
        {
            try
            {
                Profesor existingProfesor = _colegioCRUDContext.Profesor.Find(id);

                if (existingProfesor == null)
                {
                    return NotFound("Profesor not found");
                }

                _colegioCRUDContext.Profesor.Remove(existingProfesor);
            
                var delete = _colegioCRUDContext.SaveChanges();

                if (delete >= 1)
                {

                    return Ok();
                }
                else {
                    return NotFound("No Profesor Delete"); 
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }
        // ProfesorController.cs

        [HttpGet(Name = "GetAllProfesores")]
        public IActionResult GetAll()
        {
            try
            {
                List<Profesor> profesores = _colegioCRUDContext.Profesor.ToList();

                if (profesores == null || profesores.Count == 0)
                {
                    return NotFound("No Profesores found");
                }

                List<Profesor1> profesores1 = new List<Profesor1>();

                foreach (var profesor in profesores)
                {
                    profesores1.Add(new Profesor1
                    {
                        Id = profesor.Id,
                        Nombre = profesor.Nombre,
                        Apellido = profesor.Apellido,
                        Genero = profesor.Genero
                    });
                }

                return Ok(profesores1);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
