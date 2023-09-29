using CTS_ReturnsApp.Interfaces;

namespace CTS_ReturnsApp.UnitOfWork
{
    public interface IUnitOfWorkDb2
    {
        IDb2Repository Db2 { get; }
    }
}
