using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitoringNetCore.Areas.Identity.Data;

[assembly: HostingStartup(typeof(MonitoringNetCore.Areas.Identity.IdentityHostingStartup))]
namespace MonitoringNetCore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MonitoringNetCoreIdentityDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MonitoringNetCoreIdentityDbContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<MonitoringNetCoreIdentityDbContext>();
            });
        }
    }
}
