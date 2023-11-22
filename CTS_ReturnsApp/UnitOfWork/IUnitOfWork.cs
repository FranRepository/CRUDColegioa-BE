using CTS_ReturnsApp.Interfaces;

namespace CTS_ReturnsApp.UnitOfWork
{
    public class IUnitOfWork
    {
        IProductionRepository? Production { get; }
    }
}
