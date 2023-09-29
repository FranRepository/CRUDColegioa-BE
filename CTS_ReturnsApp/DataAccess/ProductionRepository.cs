using CTS_ReturnsApp.Interfaces;
using CTS_ReturnsApp.Models;

namespace CTS_ReturnsApp.DataAccess
{
    public class ProductionRepository : Repository<CtsOu>, IProductionRepository
    {
        public ProductionRepository(string connectionString) : base(connectionString) { }

    }
}
