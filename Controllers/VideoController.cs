using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;
using MonitoringNetCore.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MonitoringNetCore.Controllers
{
    public class UploadedVideo
    {
        // public string ImageCaption { set;get; }
        // public string ImageDescription { set;get; }
        public IFormFile FormFile { set; get; }
    }
    [Authorize]
    public class VideoController : Controller
    {
        private readonly DataBaseContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger _logger;
        private readonly AiService _aiService;
        private readonly VideoFileService _videoFileService;
        IAmazonS3 S3Client { get; set; }
        public VideoController(DataBaseContext context,
            IWebHostEnvironment environment,
            IAmazonS3 s3Client,
            ILogger<VideoController> logger,
            AiService aiService,
            VideoFileService videoVideoFileService
            )
        {
            _context = context;
            hostingEnvironment = environment;
            this.S3Client = s3Client;
            _logger = logger;
            _aiService = aiService;
            _videoFileService = videoVideoFileService;
        }

        // GET: Video
        public async Task<IActionResult> Index(int? pageNumber,int? cameraId,string? userId,bool? isProcessed)
        {
            ViewBag.title = "ویدئوها";
            ViewBag.menuNavigation = "پیشخوان";
            ViewBag.menuItem = "ویدئوها";
            ViewBag.activeNavigation = "navigationDashboard";
            ViewBag.activeItem = "itemVideos";

            var videos = _videoFileService.GetVideosQuery();
            
            ViewData["cameraId"] = cameraId;
            ViewData["userId"] = userId;
            ViewData["isProcessed"] = isProcessed;

            
            if (cameraId != null)
                videos = videos.Where(file => file.CameraId == cameraId);
            if (!String.IsNullOrEmpty(userId))
                videos = videos.Where(file => file.UserId == userId);
            
            if (isProcessed != null)
                videos = videos.Where(file => file.IsProcessed == isProcessed);
            videos = videos.OrderByDescending(s => s.UploadDate);
            return View(await PaginatedList<VideoFile>.CreateAsync(videos.AsNoTracking(), pageNumber ?? 1, 9));
        }

        // GET: Video/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            var videoFile = await _videoFileService.GetVideo(id);
            if (videoFile == null)
            {
                return NotFound();
            }

            return View(videoFile);
        }
        public async Task<IActionResult> SideBySide(int? id)
        {
            
            var videoFile = await _videoFileService.GetVideo(id);
            if (videoFile == null)
            {
                return NotFound();
            }

            return View(videoFile);
        }
        
        
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Video/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50L * 1024L * 1024L* 1024L)]   
        [RequestFormLimits(MultipartBodyLengthLimit = 50L * 1024L * 1024L* 1024L)]
        public async Task<IActionResult> Upload(UploadedVideo videoFile)
        {

            _logger.LogInformation("Uploading File");
            if (videoFile.FormFile != null)
            {
                var uniqueFileName = videoFile.FormFile .FileName;
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine("/tmp",uniqueFileName);
                videoFile.FormFile.CopyTo(new FileStream(filePath, FileMode.Create));
                IdentityUser user = _context.Users.Where(user => user.Email == User.Identity.Name).ToList().First();
                await _videoFileService.AddVideo(filePath, user.Id);
                return RedirectToAction(nameof(Index));
                //to do : Save uniqueFileName  to your db table   
            }
            return View();
        }


        
        
        
        
        
        
        
        // GET: Video/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoFile = await _context.VideoFile.FindAsync(id);
            if (videoFile == null)
            {
                return NotFound();
            }
            return View(videoFile);
        }

        // POST: Video/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Path,UploadDate")] VideoFile videoFile)
        {
            if (id != videoFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videoFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoFileExists(videoFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(videoFile);
        }

        // GET: Video/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var videoFile = await _context.VideoFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videoFile == null)
            {
                return NotFound();
            }

            return View(videoFile);
        }

        // POST: Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var videoFile = await _context.VideoFile.FindAsync(id);
            _context.VideoFile.Remove(videoFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        // POST: Video/Delete/5

        public async Task<IActionResult> BeginProcess(int id)
        {
            var videoFile = await _context.VideoFile.FindAsync(id);
            var job = new VideoProcessJob()
            {
                Status = JobStatus.Waiting,
                VideoId = videoFile.Id,
            };
            var process = _context.Add(job);
            await _context.SaveChangesAsync();
            TempData["msg"] = "در صف پردازش قرار گرفت";
            return RedirectToAction(nameof(Details),new {id = id});
        }
        
        private bool VideoFileExists(int id)
        {
            return _context.VideoFile.Any(e => e.Id == id);
        }
    }
}
