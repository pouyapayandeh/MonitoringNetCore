
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Monitoring.Common;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace MonitoringNetCore.Controllers
{
    public class CameraList
    {
        public  IEnumerable<SelectListItem> Cameras { get; set; }
        public int[] CameraIds { get; set; }
    }
    [Authorize]
	public class CameraController : Controller
	{
        private readonly CameraService _cameraService;
        private readonly Settings settings;
        // GET: Camera
        public CameraController(CameraService cameraService, Settings configuration)
        {
            settings = configuration;
            _cameraService = cameraService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _cameraService.GetAllCameras());
        }

        
        public async Task<IActionResult> CreateTile()
        {
            var cameraList = await _cameraService.GetAllCameras();
            var _CameraList = new CameraList();
           _CameraList.Cameras = cameraList.Select(camera => new SelectListItem()
            {
                Text = camera.Name,
                Value = camera.Id.ToString()
            });
            return View(_CameraList);
        }
        [HttpPost]
        public async Task<IActionResult> Tile(CameraList cameraList)
        {

            return View(cameraList);
        }
        
        // GET: camera/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var camera = await _cameraService.GetCamera(id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        public async Task<IActionResult> StartSideBySide(int? id)
        {
            
            await _cameraService.StartRealtimeProcess(id);
            return RedirectToAction(nameof(SideBySide),new { id = id });
        }



        public async Task<IActionResult> SideBySide(int? id)
        {

            
            var camera = await _cameraService.GetCamera(id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        public async Task<IActionResult> StopAll()
        {
            await _cameraService.StopRealtimeProcessingForAll();

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Create([Bind("Name,Type,Url")] Camera camera)
        {
            if (ModelState.IsValid)
            {
                await _cameraService.AddCamera(camera);
                return RedirectToAction(nameof(Index));
            }
            return View(camera);
        }

        // GET: camera/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camera = await _cameraService.GetCamera(id);
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
        public async Task<IActionResult> Edit(int id,  Camera camera)
        {
            if (id != camera.Id)
            {
                return NotFound();
            }
            if (!_cameraService.CameraExists(camera.Id))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _cameraService.UpdateCamera(camera);
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

            var camera = await _cameraService.GetCamera(id);
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
            await _cameraService.RemoveCamera(id);
            return RedirectToAction(nameof(Index));
        }


    }
}