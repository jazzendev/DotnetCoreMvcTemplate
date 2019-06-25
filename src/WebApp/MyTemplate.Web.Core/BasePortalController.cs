using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Web.Core
{
    [Authorize]
    public class BasePortalController : BaseController
    {
        private readonly ILogger _logger;

        public BasePortalController(ILogger logger) : base(logger)
        {
            _logger = logger;
        }
    }
}
