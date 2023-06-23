using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MonitoringNetCore.Controllers;

public class PanelController : Controller
{
    IAmazonS3 S3Client { get; set; }
    private readonly IHostingEnvironment hostingEnvironment;
    public PanelController(IHostingEnvironment environment,IAmazonS3 s3Client)
    {
        hostingEnvironment = environment;
        this.S3Client = s3Client;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        ViewBag.title = "ویدئوها";
        ViewBag.menuNavigation = "پیشخوان";
        ViewBag.menuItem = "ویدئوها";
        ViewBag.activeNavigation = "navigationDashboard";
        ViewBag.activeItem = "itemVideos";

        ListObjectsResponse result = await S3Client.ListObjectsAsync("uploads");

        return View(result.S3Objects);
    }
}