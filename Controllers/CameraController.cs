using System;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitoring.Presistence.Contexts;
using Monitoring.Site.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Monitoring.Presistence.Contexts;
using Monitoring.Site.Domain.Entities;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace MonitoringNetCore.Controllers
{
	public class CameraController : Controller
	{
        private readonly DataBaseContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        public CameraController(DataBaseContext context, IHostingEnvironment environment)
        {
            _context = context;
            hostingEnvironment = environment;
        }

        // GET: Camera
        public async Task<IActionResult> Index()
        {
            return View(await _context.Camera.ToListAsync());
        }

        // GET: camera/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camera = await _context.Camera
                .FirstOrDefaultAsync(m => m.Id == id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        // GET: camera/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Video/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,InsertedAt,Type,Url")] Camera camera)
        {
            if (ModelState.IsValid)
            {
                _context.Add(camera);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(camera);
        }
        //public IActionResult Upload()
        //{
        //    return View();
        //}

        //// POST: Video/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upload(UploadedVideo videoFile)
        //{
        //    if (videoFile.FormFile != null)
        //    {
        //        var uniqueFileName = videoFile.FormFile.FileName;
        //        var uploads = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
        //        var filePath = Path.Combine(uploads, uniqueFileName);
        //        // videoFile.FormFile.CopyTo(new FileStream(filePath, FileMode.Create));

        //        using (var newMemoryStream = new MemoryStream())
        //        {
        //            videoFile.FormFile.CopyTo(newMemoryStream);

        //            var uploadRequest = new TransferUtilityUploadRequest
        //            {
        //                InputStream = newMemoryStream,
        //                Key = videoFile.FormFile.FileName,
        //                BucketName = "uploads",
        //                CannedACL = S3CannedACL.PublicRead,

        //            };

        //            var fileTransferUtility = new TransferUtility(S3Client);
        //            await fileTransferUtility.UploadAsync(uploadRequest);
        //        }


        //        var video = new VideoFile
        //        {
        //            Path = filePath,
        //            UploadDate = DateTime.Now
        //        };
        //        _context.Add(video);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //        //to do : Save uniqueFileName  to your db table   
        //    }
        //    return View();
        //}









        // GET: camera/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camera = await _context.Camera.FindAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return View(camera);
        }

        // POST: Video/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,InsertedAt,Type,Url")] Camera camera)
        {
            if (id != camera.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(camera);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CameraExists(camera.Id))
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
            return View(camera);
        }

        // GET: Video/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camera = await _context.Camera
                .FirstOrDefaultAsync(m => m.Id == id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        // POST: Video/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var camera = await _context.Camera.FindAsync(id);
            _context.Camera.Remove(camera);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CameraExists(int id)
        {
            return _context.Camera.Any(e => e.Id == id);
        }
    }
}