using ColegioCRUDApp.Interfaces;
using ColegioCRUDApp.UnitOfWork;
namespace ColegioCRUDApp.DataAccess
{
    public class UnitWork : IUnitOfWork
    {
        public UnitWork(string connectionString)
        {
            Alumno = new ProductionRepositoryAlumno(connectionString);
            AlumnoGrado = new ProductionRepositoryAlumnoGrado(connectionString);
            Profesor = new ProductionRepositoryProfesor(connectionString);
            Grado = new ProductionRepositoryGrado(connectionString);
        }

        public IProductionRepositoryAlumno Alumno { get; }
        public IProductionRepositoryAlumnoGrado AlumnoGrado { get; }
        public IProductionRepositoryProfesor Profesor { get; }
        public IProductionRepositoryGrado Grado { get; }
    }
}

