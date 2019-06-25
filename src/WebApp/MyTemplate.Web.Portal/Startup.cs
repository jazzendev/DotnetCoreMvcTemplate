using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using MyTemplate.Core.Repository;
using MyTemplate.Core.Security;
using MyTemplate.Domain.Portal.DomainModel;
using MyTemplate.Domain.Portal.Model;
using MyTemplate.Domain.Portal.Repository;
using MyTemplate.Web.Core.Models;
using MyTemplate.Web.Core.Policies;
using MyTemplate.Web.Portal.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace MyTemplate.Web.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // setup loging
            ILoggerFactory loggerFactory = new LoggerFactory().AddNLog();
            NLog.LogManager.LoadConfiguration($"nlog.config");
            services.AddSingleton(loggerFactory);
            services.AddLogging(); // Allow ILogger<T>

            // default mvc service settings
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        //options.Cookie.Domain = "www.jingtengtech.com";
                        options.Cookie.Expiration = TimeSpan.FromMinutes(10);
                    });

            services.AddAuthorization(options => options.AddPolicy("SuperAdminOnly",
                policy => policy.Requirements.Add(new RoleRequirement(new PortalRoles[] { PortalRoles.SuperAdmin })))
                );

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set let session expire after 10 minutes if request idle
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.HttpOnly = true;
            });

            // register authorization handler
            services.AddSingleton<IAuthorizationHandler, RolePolicyHandler>();

            // initiate core
            services.AddSingleton<IMapper>(ConfigureMapper());
            services.AddSingleton(new SimplePasswordHasher());

#if (DEBUG)
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory(Configuration, "DefaultConnection")); //DefaultConnection
#endif
#if (RELEASE)
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory(Configuration, "ProductionConnection"));
#endif

            // inject repositories
            services.AddScoped<IPortalLoginRepository, PortalLoginRepository>();

            // inject principal for specific classes
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(
                provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

            // Have to manually new DefaultSettings because of the .net core's shitty design
            var initConfiguration = new MyTemplate.Core.DefaultSettings(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }

        public IMapper ConfigureMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // db to dto
                cfg.CreateMap<PortalLoginDto, PortalLogin>().ReverseMap();

                //dto to view model
                cfg.CreateMap<PortalLoginDto, PortalLoginViewModel>().ReverseMap();
            });
            var mapper = config.CreateMapper();
            return mapper;
        }
    }
}
