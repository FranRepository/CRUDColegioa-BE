using ColegioCRUDApp.Interfaces;
using ColegioCRUDApp.Models;

namespace ColegioCRUDApp.DataAccess
{
    public class ProductionRepositoryAlumno : Repository<Alumno>, IProductionRepositoryAlumno
    {
        public ProductionRepositoryAlumno(string connectionString) : base(connectionString) { }

    }
    public class ProductionRepositoryGrado : Repository<Grado>, IProductionRepositoryGrado
    {
        public ProductionRepositoryGrado(string connectionString) : base(connectionString) { }

    }
    public class ProductionRepositoryProfesor : Repository<Profesor>, IProductionRepositoryProfesor
    {
        public ProductionRepositoryProfesor(string connectionString) : base(connectionString) { }

    }
    public class ProductionRepositoryAlumnoGrado : Repository<AlumnoGrado>, IProductionRepositoryAlumnoGrado
    {
        public ProductionRepositoryAlumnoGrado(string connectionString) : base(connectionString) { }

    }
}
