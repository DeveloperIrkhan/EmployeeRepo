using DAL.DbContexts;
using DAL.Repository;
using Employee.DOMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WebApp
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<UserSignUp, IdentityRole>(option =>
            {
                option.Password.RequiredLength = 5;
                option.Password.RequireLowercase = false;
                option.Password.RequireDigit = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireUppercase = true;
                

            }) 
                .AddEntityFrameworkStores<EmpDbContext>();

            services.AddDbContextPool<EmpDbContext>
                (
                dbContext =>
                dbContext.UseSqlServer
                (
                    _config.GetConnectionString("EmployeeDbConnection")
                ));

            services.AddMvc(action =>
            {
                action.EnableEndpointRouting = false;
                //used For Globle Autherization
                //var policy = new AuthorizationPolicyBuilder()
                //                .RequireAuthenticatedUser()
                //                .Build();
                //action.Filters.Add(new AuthorizeFilter(policy));
            });

            services.ConfigureApplicationCookie(cokiee => 
            {
                cokiee.LoginPath = "/Auth/SignIn/";
                cokiee.AccessDeniedPath = "/Admin/AccessDenied/";
                cokiee.Cookie.MaxAge = TimeSpan.FromMinutes(15);
            });


            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc(Route =>
            {
                Route.MapRoute
                (
                    name: "IRkhanApp",
                    template: "{controller}/{action}/{id?}",
                    defaults: new
                    {
                        controller = "Home",
                        action = "Index"
                    });
            });
        }
    }
}
