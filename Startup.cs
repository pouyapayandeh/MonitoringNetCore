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
using Monitoring.Application.Services.Books.Commands.AddBook;
using Monitoring.Application.Services.Books.Queries.GetBook;
using Amazon.S3;
using Microsoft.Data.SqlClient;
using Monitoring.Common;

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

            // Settings
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var settings = new Settings
            {
                AWSEndpoint = Environment.GetEnvironmentVariable("AWS_ENDPOINT") ?? "http://127.0.0.1:9000/",
                AWSAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? "EkDiyryHuatO2kRS",
                AWSSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY_ID") ?? "13Qcg4oiPxRL4LyVhpnxQx992UmoiRJ6"
            };

            services.Configure<Settings>(options => Configuration.Bind(options));
            services.AddSingleton(settings);

            // System
            services.AddScoped<IDataBaseContext, DataBaseContext>();

            // Plate
            services.AddScoped<IGetBookService, GetBookService>();
            services.AddScoped<IAddBookService, AddBookService>();
            

            var appSetting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json",optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();


            string connectionString = appSetting["ConnectionStrings:PostgresConnection"];
            Console.WriteLine(connectionString);
            services.AddEntityFrameworkSqlServer().AddDbContext<DataBaseContext>(
                options => options.UseNpgsql(connectionString));

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
                        ServiceURL = settings.AWSEndpoint,
                        ForcePathStyle = true
                    };
                    return new AmazonS3Client(settings.AWSAccessKeyId, settings.AWSSecretAccessKey, config);
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
