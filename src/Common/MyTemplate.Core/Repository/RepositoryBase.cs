using System;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Core.Repository
{
    public class RepositoryBase : IRepository
    {
        private readonly ILogger _logger;

        public RepositoryBase(ILogger logger)
        {
            _logger = logger;
        }
    }
}
