using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Common;
using MonitoringNetCore.Common;
using MonitoringNetCore.Services;

namespace MonitoringNetCore.Controllers;


public class StatusController : Controller
{

    private readonly CameraService _cameraService;
    private readonly Settings _settings;
    public StatusController(CameraService cameraService,Settings settings)
    {
        _cameraService = cameraService;
        _settings = settings;
    }

    // GET
    public string MediaMtx()
    {
        return _cameraService.GetYamlContent();
    }
    
    public string Settings()
    {
        
        var jsonString = JsonSerializer.Serialize(_settings);
        return jsonString;
    }
}