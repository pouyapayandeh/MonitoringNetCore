using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;
using MonitoringNetCore.Services;

namespace MonitoringNetCore.Controllers;
[Authorize]
public class JobsController : Controller
{
    private readonly AiService _aiService;
    // GET
    public JobsController(AiService aiService)
    {
        _aiService = aiService;
    }

    public async Task<IActionResult> Index(int? pageNumber)
    {

        return View(await PaginatedList<VideoProcessJob>.CreateAsync(_aiService.GetJobsQuery().AsNoTracking(), 
            pageNumber ?? 1, 10));
    }
}