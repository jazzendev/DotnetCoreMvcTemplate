using AutoMapper;
using MyTemplate.Domain.Portal.DomainModel;
using MyTemplate.Domain.Portal.Repository;
using MyTemplate.Web.Portal.Models;
using MyTemplate.Web.Core;
using MyTemplate.Web.Core.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MyTemplate.Web.Portal.Controllers
{
    [Authorize(Policy = "SuperAdminOnly")]
    public class LoginController : BasePortalController
    {
        private readonly IPortalLoginRepository _pr;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public LoginController(IPortalLoginRepository portalLoginRepository,
                               ILogger<LoginController> logger,
                               IMapper mapper) : base(logger)
        {
            _pr = portalLoginRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var logins = _pr.GetPortalLogins(new PortalLoginQuery() { });
            return View("Index", logins);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, NoStore = true)]
        public IActionResult GetMorePortalLogins(PortalLoginQuery query)
        {
            var logins = _pr.GetPortalLogins(query);

            return PartialView("_portalLoginList", logins);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, NoStore = true)]
        public IActionResult Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Roles = _pr.GetRoles();
                return View(new PortalLoginViewModel());
            }

            var dto = _pr.GetPortalLogin(id);
            if (dto == null)
            {
                return NotFound();
            }

            PortalLoginViewModel viewModel = _mapper.Map<PortalLoginViewModel>(dto);
            ViewBag.Roles = _pr.GetRoles();
            return View("Upsert", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(PortalLoginViewModel viewModel)
        {
            var result = new ResultViewModel() { IsSuccess = false };
            if (!_pr.CheckUsernameAvaiable(viewModel.Id, viewModel.Username))
            {
                ModelState.AddModelError("Username", "用户名已被使用。");
            }
            if (viewModel.RoleList == null || viewModel.RoleList.Count() == 0)
            {
                ModelState.AddModelError("RoleList", "必须选择权限。");
            }

            if (ModelState.IsValid)
            {
                bool isNew = string.IsNullOrEmpty(viewModel.Id);
                PortalLoginDto dto;

                if (isNew)
                {
                    dto = _pr.CreatePortalLogin(viewModel.Username, viewModel.Password, viewModel.RoleList, true);
                }
                else
                {
                    // if password changed, then user need to change password when login
                    var isPasswordToChange = viewModel.IsPasswordChanged;

                    dto = _pr.UpdatePortalLogin(viewModel.Id, viewModel.Username, viewModel.Password, viewModel.RoleList, isPasswordToChange, viewModel.IsPasswordChanged, false);
                }

                if (dto != null)
                {
                    viewModel = _mapper.Map<PortalLoginViewModel>(dto);
                    result.IsSuccess = true;
                }
            }

            var roles = _pr.GetRoles();
            ViewBag.Roles = roles;
            ViewBag.Result = result;

            return View("Upsert", viewModel);
        }
    }
}