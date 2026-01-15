using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Gestion_bibliot.DAL.Interfaces;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();

        public T GetById(object id) => _dbSet.Find(id);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate).ToList();

        public void Add(T entity) => _dbSet.Add(entity);

        public void AddRange(IEnumerable<T> entities) => _dbSet.AddRange(entities);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void Remove(object id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null) _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
