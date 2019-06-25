using Microsoft.Extensions.Configuration;

namespace MyTemplate.Core
{
    public class DefaultSettings
    {
        private static IConfiguration _configuration;

        public static DefaultSettings Instance { get; private set; }

        public DefaultSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static int DefaultPageSize
        {
            get
            {
                return int.Parse(_configuration["defaults:DefaultPageSize"]);
            }
        }
    }
}
