using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MyTemplate.Domain.Portal.DomainModel;
using MyTemplate.Domain.Portal.Repository;
using MyTemplate.Web.Core;
using MyTemplate.Web.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Web.Portal.Controllers
{
    public class AccountController : BasePortalController
    {
        private readonly IPortalLoginRepository _pr;
        private readonly ILogger _logger;

        public AccountController(IPortalLoginRepository portalLoginRepository,
                                 ILogger<AccountController> logger) : base(logger)
        {
            _pr = portalLoginRepository;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {

            //Get all users
            var users = _pr.GetPortalLogins(new PortalLoginQuery()
            {
                IsValid = null
            });

            if (users.Total == 0)
            {
                ViewBag.IsCreated = false;
                return View(new LoginViewModel());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(LoginViewModel model)
        {
            var users = _pr.GetPortalLogins(new PortalLoginQuery()
            {
                IsValid = null
            });

            if (users.Total == 0)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.IsCreated = false;
                    return View(model);
                }

                _pr.CreatePortalLogin(model.Username, model.Password, new string[] { "1" }, false);
                ViewBag.IsCreated = true;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, NoStore = true)]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var combination = model.Username.Split("@");
            //var username = combination[0];
            //if (combination.Length == 1)
            //{
            //    if (username == "jingteng")
            //    {
            //        // it's correct, superadmin account
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("Username", "用户名错误。");
            //        return View(model);
            //    }
            //}
            //else
            //{
            //    if (combination[1].ToLower() != "jingteng")
            //    {
            //        ModelState.AddModelError("Username", "用户名错误。");
            //        return View(model);
            //    }
            //}

            var user = _pr.VerifyPortalLogin(model.Username, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("Password", "用户名或密码错误。");
                return View(model);
            }

            var roles = _pr.GetPortalLogin(user.Id).Roles;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, roles)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.IsPasswordToChange)
            {
                return RedirectToRoute(new { Controller = "Account", Action = "ChangePassword" });
            }

            return RedirectToRoute(new { Controller = "Home", Action = "Index" });
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var login = _pr.GetPortalLogin(id);

            var viewModel = new ChangePasswordViewModel()
            {

            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel viewModel)
        {
            var result = new ResultViewModel() { IsSuccess = false };
            if (ModelState.IsValid)
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                result.IsSuccess = _pr.ChangePassword(id, viewModel.OldPassword, viewModel.NewPassword);
            }

            ViewBag.Result = result;
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        [ResponseCache(Duration = 0, NoStore = true)]
        public IActionResult CurrentLogin()
        {
            ViewBag.Id = _pr.GetCurrentLogin();
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}