using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MonitoringNetCore.Services;

namespace MonitoringNetCore.Controllers;
public class NewLicense
{

    [Required]
    [Display(Name = "License")]
    public string? License { get; set; }
    
    
    [Display(Name = "Status")]
    public bool? Status { get; set; }

}
public class LicenseController : Controller
{
    private readonly LicenseService _licenseService;

    public LicenseController(LicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    public async Task<IActionResult> Index()
    {
        var license = await _licenseService.GetLicense();
        var retLicense = new NewLicense
        {
            License = license?.Value,
            Status = (await _licenseService.IsLicensed())
        };
        return View(retLicense);
    }
    // GET: Video/Create
    public async  Task<IActionResult> Create()
    {
        return View();
    }

    // POST: Video/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("License")] NewLicense  newLicense)
    {

        await _licenseService.AddLicense(newLicense.License);
        return RedirectToAction(nameof(Index));
    }
}