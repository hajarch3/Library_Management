using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Gestion_bibliot.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void Remove(object id);
        void Update(T entity);
    }
}
