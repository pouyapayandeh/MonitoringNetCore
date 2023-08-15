using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Monitoring.Common;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;
using Standard.Licensing.Validation;
using StandardLicense = Standard.Licensing.License;

namespace MonitoringNetCore.Services;

public class LicenseService
{
    private readonly DataBaseContext _context;

    // private readonly ILogger _logger;
    private readonly Settings _settings;

    public LicenseService(DataBaseContext context)
    {
        _context = context;

    }

    private bool isValid(string license)
    {
        var assembly = Assembly.GetExecutingAssembly();
        Stream resource = assembly.GetManifestResourceStream("MonitoringNetCore.publicKey");
        StreamReader reader = new StreamReader(resource);
        var pubKey = reader.ReadLine();
        try
        {
            var newlicense = StandardLicense.Load(Base64UrlEncoder.Decode(license));
            var validationFailures = newlicense.Validate().Signature(pubKey).AssertValidLicense();
            if (validationFailures.Any())
                return false;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    public async Task AddLicense(string license)
    {
        _context.Licenses.RemoveRange(_context.Licenses);
        if (isValid(license))
        {
            var newLicense = new License
            {
                Value = license
            };
            var process = _context.Add(newLicense);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<bool> IsLicensed()
    {
        var license = _context.Licenses.SingleOrDefault();
        return isValid(license?.Value);
    }

    public async Task<License> GetLicense()
    {
        var license = _context.Licenses.SingleOrDefault();
        return license;
    }
}

