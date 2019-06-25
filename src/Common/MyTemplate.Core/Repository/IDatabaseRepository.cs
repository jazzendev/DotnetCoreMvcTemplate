using System;
using System.Collections.Generic;
using MyTemplate.Core.Model;

namespace MyTemplate.Core.Repository
{
    public interface IDatabaseRepository<T, TKey> : IRepository
        where T : IDatabaseModel<TKey>
    {
        IEnumerable<T> GetAll();
        T Find(TKey key);
        T Insert(T entity);
        bool Update(T entity);
        bool Disable(TKey key);
        bool Enable(TKey key);
        bool Delete(T entity);
    }
}
