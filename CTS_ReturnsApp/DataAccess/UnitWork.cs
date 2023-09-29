using CTS_ReturnsApp.Interfaces;
using CTS_ReturnsApp.UnitOfWork;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CTS_ReturnsApp.DataAccess
{
    public class UnitWork : IUnitOfWork, IUnitOfWorkDb2
    {

        public UnitWork(string connectionString)
        {
            Production = new ProductionRepository(connectionString);
            Db2 = new Db2Repository(connectionString);
        }
        public IProductionRepository Production { get; set; }
        public IDb2Repository Db2 { get; set; }

    }
}
