using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MonitoringNetCore.Persistence.Contexts;
using MonitoringNetCore.Services;
using Standard.Licensing;

namespace Monitoring.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("gen-keypair"))
            {
                var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create(); 
                var keyPair = keyGenerator.GenerateKeyPair(); 
                var privateKey = keyPair.ToEncryptedPrivateKeyString("6mML7vhZGiOv2$cGriVB^YBwEWG6aSmOzyko41Zet*GVqL%zqN");  
                var publicKey = keyPair.ToPublicKeyString();
                Console.WriteLine(privateKey);
                Console.WriteLine(publicKey);
                return;
            }
            if (args.Length > 0 && args[0].Equals("new-license"))
            {
                Console.Write("Enter Customer Name:");
                var customerName = Console.ReadLine();
                Console.Write("Enter Private Key:");
                var privateKey = Console.ReadLine();
                Console.Write("Enter Private Key PassPhrase:");
                var passPhrase = Console.ReadLine();
                
                DateTime expiryDate  = DateTime.UtcNow.Date.AddDays(90);
                License newLicense = License.New()
                    .WithUniqueIdentifier(Guid.NewGuid())
                    .ExpiresAt(expiryDate)
                    .LicensedTo((c) => c.Name = customerName)
                    .CreateAndSignWithPrivateKey(privateKey, passPhrase);
                
                string licenseKey = Base64UrlEncoder.Encode(newLicense.ToString());

                Console.WriteLine(licenseKey);
                return;
            }

            var host = CreateHostBuilder(args).Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
                db.Migrate();

                var cameraService = scope.ServiceProvider.GetRequiredService<CameraService>();
                cameraService.RebuildYamlFile().Wait();
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
