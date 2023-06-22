using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Monitoring.Presistence.Contexts;
using Monitoring.Site.Domain.Entities;
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
        private readonly IHostingEnvironment hostingEnvironment;
        IAmazonS3 S3Client { get; set; }
        public VideoController(DataBaseContext context,IHostingEnvironment environment,IAmazonS3 s3Client)
        {
            _context = context;
            hostingEnvironment = environment;
            this.S3Client = s3Client;
        }

        // GET: Video
        public async Task<IActionResult> Index()
        {
            return View(await _context.VideoFile.ToListAsync());
        }

        // GET: Video/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Video/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Video/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Path,UploadDate")] VideoFile videoFile)
        {
            if (ModelState.IsValid)
            {
                videoFile.UploadDate = DateTime.SpecifyKind(videoFile.UploadDate, DateTimeKind.Utc);
                _context.Add(videoFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Upload(UploadedVideo videoFile)
        {
            if (videoFile.FormFile != null)
            {
                var uniqueFileName = videoFile.FormFile .FileName;
                var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploads,uniqueFileName);
                // videoFile.FormFile.CopyTo(new FileStream(filePath, FileMode.Create));
                
                using (var newMemoryStream = new MemoryStream())
                {
                    videoFile.FormFile.CopyTo(newMemoryStream);

                    
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key =  videoFile.FormFile .FileName,
                        BucketName = "uploads",
                        CannedACL = S3CannedACL.PublicRead,
                        
                    };

                    var fileTransferUtility = new TransferUtility(S3Client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
                
                
                var video = new VideoFile
                {
                    Path = filePath,
                    UploadDate = DateTime.UtcNow
                };
                _context.Add(video);
                await _context.SaveChangesAsync();
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

        private bool VideoFileExists(int id)
        {
            return _context.VideoFile.Any(e => e.Id == id);
        }
    }
}
