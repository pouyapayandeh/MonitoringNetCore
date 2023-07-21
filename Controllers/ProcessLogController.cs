using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Presistence.Contexts;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.ComponentModel.DataAnnotations;

namespace MonitoringNetCore.Controllers;

public class ProcessLogController : Controller
{
    private readonly DataBaseContext _context;
    public ProcessLogController(DataBaseContext context)
    {
        _context = context;
    }
    // GET
    public IActionResult Index()
    {
        ViewBag.title = "پردازش ها";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "پردازش ها";
        ViewBag.activeNavigation = "navigationDashboard";
        ViewBag.activeItem = "itemProcessLog";
        return View(_context.ProcessLogs.ToList());
    }
  
    // // GET: Video/Delete/5
    // public async Task<IActionResult> Delete(string? id)
    // {
    //     if (id == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var user = _userManager.Users.Where(user => user.Id.Equals(id)).First();
    //     IdentityResult result = await  _userManager.DeleteAsync(user);
    //     if (! result.Succeeded)
    //     {
    //         return NotFound();
    //     }
    //
    //     return RedirectToAction(nameof(Index));
    // }
}