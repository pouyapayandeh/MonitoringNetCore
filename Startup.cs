using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoring.Presistence.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.IO;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
namespace Monitoring.Site
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

            // // System
            // services.AddScoped<IDataBaseContext, DataBaseContext>();
            //
            // // Plate
            // services.AddScoped<IGetBookService, GetBookService>();
            // services.AddScoped<IAddBookService, AddBookService>();
            //

            var appSetting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json",optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();
            

            string connectionString = appSetting["ConnectionStrings:DefaultConnection"];
            // services.AddEntityFrameworkSqlServer().AddDbContext<DataBaseContext>(
            //     options => options.UseSqlite(@"DataSource=mydatabase.db;"));
            
            services.AddDbContext<DataBaseContext>(options =>
                options.UseSqlite(@"DataSource=mydatabase.db;"));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<DataBaseContext>();
            

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services
                .AddScoped<IAmazonS3>(p => {
                    var config = new AmazonS3Config
                    {
                        ServiceURL = "http://172.30.21.175:9000/",
                        ForcePathStyle = true
                    };
                    return new AmazonS3Client("EkDiyryHuatO2kRS", "13Qcg4oiPxRL4LyVhpnxQx992UmoiRJ6", config);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

        }
    }
}
