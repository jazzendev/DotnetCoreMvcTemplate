using System;
using Dapper.Contrib.Extensions;

namespace MyTemplate.Core.Model
{
    public class MonitorModel<TKey> : IMonitorModel<TKey>
    {
        [ExplicitKey]
        public TKey Id { get; set; }

        public string CreatorId { get; set; }
        public string EditorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastEditTime { get; set; }
        public bool IsValid { get; set; }
    }
}
