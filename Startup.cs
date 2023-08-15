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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.IO;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Microsoft.Data.SqlClient;
using Monitoring.Common;
using Monitoring.Common.Middleware;
using MonitoringNetCore.Common;
using MonitoringNetCore.Persistence.Contexts;
using MonitoringNetCore.Services;
using MonitoringNetCore.Services.BackgroundServices;

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


            var settings = new Settings();
            Configuration.Bind(settings);
            services.AddSingleton(settings);
            
            services.AddDbContext<DataBaseContext>(
                options => options.UseNpgsql(settings.ConnectionStrings.PostgresConnection));


            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;

                }).AddRoles<IdentityRole>()
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
                        ServiceURL = settings.AWS.Endpoint,
                        ForcePathStyle = true
                    };
                    return new AmazonS3Client(settings.AWS.AccessKeyId, settings.AWS.SecretAccessKey, config);
                });
            
            services.AddScoped<AiService, AiService>();
            services.AddScoped<LicenseService, LicenseService>();
            services.AddScoped<CameraService, CameraService>();
            services.AddScoped<VideoFileService, VideoFileService>();
            services.AddHostedService<CameraRecordHostedService>();
            services.AddHostedService<AiJobHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var _userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var _roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var _task = _roleManager.RoleExistsAsync("Administrator");
                _task.Wait();
                
                if (!_task.Result)
                {
                    _roleManager.CreateAsync(new IdentityRole("Administrator")).Wait();
                }
                
                
                var user = new IdentityUser("admin@test.com");
                _userManager.CreateAsync(user,"admin").Wait();
                _userManager.SetUserNameAsync(user, "admin@test.com").Wait();
                _userManager.AddToRoleAsync(user, "Administrator").Wait();
                _userManager.SetEmailAsync(user,"admin@test.com").Wait();
                
            }
            
            
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

            app.UseMiddleware<LicenseMiddleware>();

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
