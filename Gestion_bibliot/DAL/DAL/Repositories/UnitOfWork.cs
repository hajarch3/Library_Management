using System;
using System.Collections.Generic;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork()
        {
            _context = new ApplicationDbContext();
        }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? new ApplicationDbContext();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = Activator.CreateInstance(typeof(Repository<>).MakeGenericType(type), _context);
                _repositories[type] = repositoryInstance;
            }
            return (IRepository<T>)_repositories[type];
        }

        public int Complete() => _context.SaveChanges();

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
