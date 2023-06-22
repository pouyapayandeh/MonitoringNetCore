using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MonitoringNetCore.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index(int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.title = "خانه";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "خانه";
        ViewBag.activeNavigation = "navigationDashboard";
        ViewBag.activeItem = "itemHome";

        // var result = _getBookService.Execute(new RequestGetBookDto
        // {
        //     PageNumber = pageNumber,
        //     PageSize = pageSize,
        // });

        return View();
    }
}