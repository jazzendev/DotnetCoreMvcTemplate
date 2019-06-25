using System;
using Dapper.Contrib.Extensions;

namespace MyTemplate.Core.Model
{
    public class DatabaseModel<TKey> : IDatabaseModel<TKey>
    {
        [ExplicitKey]
        public TKey Id { get; set; }
        public bool IsValid { get; set; }
    }
}
