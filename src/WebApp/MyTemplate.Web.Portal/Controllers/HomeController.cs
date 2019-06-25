using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyTemplate.Web.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Web.Portal.Controllers
{
    public class HomeController : BasePortalController
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public HomeController(ILogger<LoginController> logger,
                                IMapper mapper) : base(logger)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}