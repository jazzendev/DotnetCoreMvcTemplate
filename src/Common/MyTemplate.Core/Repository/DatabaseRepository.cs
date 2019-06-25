using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MyTemplate.Core.Model;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MyTemplate.Core.Repository
{
    public class DatabaseRepository : RepositoryBase
    {
        private IConnectionFactory _connectionFactory;
        private ILogger _logger;

        protected DbConnection GetConnection()
        {
            return _connectionFactory.GetConnection();
        }

        public DatabaseRepository(IConnectionFactory connectionFactory, ILogger logger)
           : base(logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        protected string GetSortingQuery(PaginationQuery query)
        {
            StringBuilder sb = new StringBuilder();
            if (query.SortingItems != null && query.SortingItems.Count() > 0)
            {
                sb.Append("ORDER BY ");

                var cols = query.SortingItems.Select(i => $"{i.Name}{(i.IsAscending ? "" : " DESC")}");

                sb.AppendJoin(',', cols);
            }
            return sb.ToString();
        }

        protected PaginationResult<R> CombinePaginationResult<R>(PaginationQuery query, IEnumerable<R> data, long totalCount)
        {
            var result = new PaginationResult<R>()
            {
                Data = data,
                Query = query,
                Page = query.Page,
                Size = query.Size,
                Total = totalCount
            };

            return result;
        }
    }
}
