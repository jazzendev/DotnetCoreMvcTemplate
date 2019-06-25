using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using MyTemplate.Core.Model;

namespace MyTemplate.Core.Repository
{
    public class MonitorRepository : DatabaseRepository
    {
        private readonly ClaimsPrincipal _principal;

        protected string CurrentUserId
        {
            get
            {
                string userId = _principal == null ? string.Empty :
                    _principal.FindFirst(ClaimTypes.NameIdentifier) == null ? string.Empty :
                    _principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException(userId, "principal for current userId not found.");
                }

                return userId;
            }
        }

        public MonitorRepository(IConnectionFactory connectionFactory, ILogger logger, IPrincipal principal)
           : base(connectionFactory, logger)
        {
            _principal = principal as ClaimsPrincipal;
        }

        protected R AppendMonitorData<R>(R entity, DateTime? timestamp = null) where R : class
        {
            var imonitor = typeof(R).GetInterface("IMonitorModel");
            if (imonitor == null || string.IsNullOrEmpty(CurrentUserId))
            {
                return entity;
            }

            var obj = IMonitorExtension.AppendMonitorData(entity as IMonitorModel, CurrentUserId, timestamp) as R;
            return obj;
        }
    }
}
