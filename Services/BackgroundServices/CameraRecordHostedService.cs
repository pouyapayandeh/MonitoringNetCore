using System.Globalization;
using Monitoring.Common;
using MonitoringNetCore.Common;

namespace MonitoringNetCore.Services.BackgroundServices;

public class CameraRecordHostedService : BackgroundService
{
    private readonly ILogger<CameraRecordHostedService> _logger;
    private readonly IServiceScopeFactory _factory;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly Settings settings;
    public CameraRecordHostedService(ILogger<CameraRecordHostedService> logger, IServiceScopeFactory factory , Settings configuration)
    {
        this.settings = configuration;
        _logger = logger;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested && 
            await timer.WaitForNextTickAsync(stoppingToken))    
        {
            try        
            {       
                await using AsyncServiceScope asyncScope = 
                    _factory.CreateAsyncScope();
                CameraService cameraService = asyncScope
                    .ServiceProvider.GetRequiredService<CameraService>();
                
                VideoFileService videoFileService = asyncScope
                    .ServiceProvider.GetRequiredService<VideoFileService>();
                
                
                string[] files = Directory.GetFiles(settings.MediaMTX.Path, "*."+settings.MediaMTX.SegmentExt);
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    var parts = filename.Split("_saved_");
                    if (parts.Length == 2)
                    {
                        _logger.LogInformation("File {} found for stream key {}",filename,parts[0]);
                        var timePart = parts[1].Split(".")[0];
                        var time = DateTime.ParseExact(timePart,"yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
                        var delta = DateTime.Now - time;
                        _logger.LogInformation("Time Delta is found {}",delta.TotalSeconds);
                        _logger.LogInformation("Time Threshold is {}",settings.MediaMTX.SegmentDuration * 2);
                        if (delta.TotalSeconds > settings.MediaMTX.SegmentDuration * 2)
                        {
                            bool isProcessed = parts[0].Contains("_process");
                            var name = parts[0];
                            if (isProcessed)
                                name = name.Split("_process")[0];
                            var camera = await cameraService.GetCamera(name);
                            
                            if (camera != null)
                            {
                                _logger.LogInformation("Try To Upload {}",filename);
                                await videoFileService.AddVideo(file, null, null, camera.Id,isProcessed);
                            }
                                
                        }
                        
                    }
                }
                
                
                
                
            }
            catch (Exception ex)        
            {
                _logger.LogError(
                    $"Failed to execute CameraRecordService: {ex.Message}.");        
            }    
        }
    }
}