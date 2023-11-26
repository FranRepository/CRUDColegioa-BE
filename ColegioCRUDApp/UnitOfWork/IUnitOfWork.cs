using ColegioCRUDApp.Interfaces;

namespace ColegioCRUDApp.UnitOfWork
{
    public class IUnitOfWork
    {
        public IProductionRepositoryAlumno? Alumno { get; }

        public IProductionRepositoryAlumnoGrado? AlumnoGrado { get; }

        public IProductionRepositoryGrado? Grado { get; }

        public IProductionRepositoryProfesor? Profesor { get; }

    }
}
