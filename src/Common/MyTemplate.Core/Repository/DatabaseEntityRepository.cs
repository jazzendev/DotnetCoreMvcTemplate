using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MyTemplate.Core.Model;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Core.Repository
{
    public class DatabaseEntityRepository<T, TKey> : DatabaseRepository, IDatabaseRepository<T, TKey> where T : class, IDatabaseModel<TKey>
    {
        private IConnectionFactory _connectionFactory;
        private ILogger _logger;

        public DatabaseEntityRepository(IConnectionFactory connectionFactory, ILogger logger)
            : base(connectionFactory, logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public virtual T Find(TKey key)
        {
            var connection = GetConnection();
            var q = connection.Get<T>(key);
            return q;
        }

        public virtual T Find(TKey key, bool? isValid = true)
        {
            var connection = GetConnection();
            var entity = connection.Get<T>(key);

            if (isValid.HasValue)
            {
                if (entity.IsValid == isValid.Value)
                {
                    return entity;
                }
                else
                {
                    return null;
                }
            }

            return entity;
        }

        public virtual IEnumerable<T> GetAll()
        {
            var connection = GetConnection();
            var entities = connection.GetAll<T>();
            return entities;
        }

        public virtual IEnumerable<T> GetAll(bool? isValid = true)
        {
            var connection = GetConnection();
            var entities = connection.GetAll<T>().Where(t => !isValid.HasValue || t.IsValid == isValid.Value);
            return entities;
        }

        public virtual T Insert(T entity)
        {
            try
            {
                var connection = GetConnection();
                connection.Insert(entity);
                return entity;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;
            }
        }

        public virtual bool Update(T entity)
        {
            try
            {
                var connection = GetConnection();
                var r = connection.Update(entity);
                return r;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

        public virtual bool Disable(TKey key)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    var entity = connection.Get<T>(key);
                    entity.IsValid = false;
                    var r = connection.Update(entity);
                    return r;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

        public virtual bool Enable(TKey key)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    var entity = connection.Get<T>(key);
                    entity.IsValid = true;
                    var r = connection.Update(entity);
                    return r;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }

        public virtual bool Delete(T entity)
        {
            try
            {
                var connection = GetConnection();
                var r = connection.Delete(entity);
                return r;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
        }
    }
}
