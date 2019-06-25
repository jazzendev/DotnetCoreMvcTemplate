using System;
using System.Collections.Generic;
using System.Text;

namespace MyTemplate.Core.Repository
{
    public interface IPaginationQuery
    {
        int Page { get; set; }
        int Size { get; set; }
    }

    public interface IPaginationResult
    {
        PaginationQuery Query { get; set; }
        int Page { get; set; }
        int Size { get; set; }
        long Total { get; set; }
    }

    public interface IPaginationResult<T> : IPaginationResult
    {
        IEnumerable<T> Data { get; set; }
    }

    public interface ISortingQuery
    {

    }
}
