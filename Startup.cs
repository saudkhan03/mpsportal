using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using portal.mps.Data;
using portal.mps.Data.Repository;
using portal.mps.Services;

namespace portal.mps
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region EF
                services.AddDbContext<mpsContext>(config => {
                    config.UseSqlServer(_config.GetConnectionString("mpsConnectionString"));
                });
                services.AddIdentity<mpsUser,IdentityRole>(config => {
                    config.User.RequireUniqueEmail=false;
                })
                .AddEntityFrameworkStores<mpsContext>()
                .AddDefaultTokenProviders();
            #endregion

            services.AddTransient<mpsSeeder>();

            #region inject repos
                services.AddScoped<IAuthRepository, AuthRepository>();
                services.AddScoped<IAdminRepository, AdminRepository>();
                services.AddScoped<IExpenseRepository,ExpenseRepository>();
                services.AddScoped<IReportsRepository, ReportsRepository>();
                services.AddScoped<IStudentsRepository,StudentsRepository>();
            #endregion
            
            services.AddTransient<IUtils, Utils>();
            services.AddTransient<IEmailSender,EmailSender>();
            
            services.ConfigureApplicationCookie(config => {
                config.LoginPath="/Auth/Login";
                config.AccessDeniedPath="/Auth/AccessDenied";
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //if(env.IsDevelopment())
            //{
                //seed the db
                
            //}
        }
    }
}
