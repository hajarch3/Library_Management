using System;

namespace Gestion_bibliot.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        int Complete();
    }
}
