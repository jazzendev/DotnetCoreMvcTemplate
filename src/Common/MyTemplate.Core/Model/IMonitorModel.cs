using System;

namespace MyTemplate.Core.Model
{
    public interface IMonitorModel<TKey> : IDatabaseModel<TKey>, IMonitorModel
    {
    }

    public interface IMonitorModel
    {
        string CreatorId { get; set; }
        string EditorId { get; set; }
        DateTime CreationTime { get; set; }
        DateTime? LastEditTime { get; set; }
    }

    public static class IMonitorExtension
    {
        public static IMonitorModel AppendMonitorData(this IMonitorModel monitor, string userId, DateTime? timestamp = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("UserId could not be null.");
            }
            if (string.IsNullOrEmpty(monitor.CreatorId))
            {
                monitor.CreatorId = userId;
                monitor.CreationTime = timestamp.HasValue ? timestamp.Value : DateTime.UtcNow;
            }
            else
            {
                monitor.EditorId = userId;
                monitor.LastEditTime = timestamp.HasValue ? timestamp.Value : DateTime.UtcNow;
            }
            return monitor;
        }
    }
}
