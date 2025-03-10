using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookStoreMVC.Utility;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using BookStoreMVC.DataAccess.Initializer;

namespace BookStoreMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>() //  Comes with, if you choose to crate a project with identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            //TODO Implement a EMail service to send emails
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration.GetSection("SendGrid")); // This will map the props to the appsettings(/secrets/env_ variables) json keys, if the names match

            // Testing some capabilities in the Category Controller
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IDataBaseInitializer, DataBaseInitializer>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            // When using Authorization, these will be the default redirects. Not Generated, must be added manually since(3.0).
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            // ---- GOOGLE AUTHETICATION ---
            //services.AddAuthentication().AddGoogle(options =>
            //{
            //    options.ClientId = Configuration.GetValue<string>("GoogleCredentials:ClientId");
            //    options.ClientSecret = Configuration.GetValue<string>("GoogleCredentials:ClientSecret");
            //});

            services.AddSession(options =>
            {

                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDataBaseInitializer dataBaseInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
           
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            dataBaseInitializer.Initializer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
