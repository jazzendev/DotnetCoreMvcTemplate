using System;
using System.Data.Common;

namespace MyTemplate.Core.Repository
{
    public interface IConnectionFactory
    {
        DbConnection GetConnection();
    }
}
