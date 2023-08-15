using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Monitoring.Common;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;

namespace MonitoringNetCore.Services.BackgroundServices;

public class AiJobHostedService : BackgroundService
{
    private readonly ILogger<CameraRecordHostedService> _logger;
    private readonly IServiceScopeFactory _factory;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly Settings settings;

    public AiJobHostedService(ILogger<CameraRecordHostedService> logger, IServiceScopeFactory factory, Settings configuration)
    {
        settings = configuration;
        _logger = logger;
        _factory = factory;
    }

    private async Task<DataBaseContext> getDataBaseContext()
    {
        await using AsyncServiceScope asyncScope =
            _factory.CreateAsyncScope();
        DataBaseContext context = asyncScope
            .ServiceProvider.GetRequiredService<DataBaseContext>();
        return context;
    }

    private async Task<VideoProcessJob> GetWaitingTask()
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            await timer.WaitForNextTickAsync())
        {
            await using AsyncServiceScope asyncScope =
                _factory.CreateAsyncScope();
            DataBaseContext context = asyncScope
                .ServiceProvider.GetRequiredService<DataBaseContext>();
            var job = await context.VideoProcessJob.AsQueryable()
                .Where(job => job.Status == JobStatus.Waiting)
                .FirstOrDefaultAsync();
            if (job != null)
                return job;

        }

        return null!;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var _job = await GetWaitingTask();
                await using AsyncServiceScope asyncScope =
                    _factory.CreateAsyncScope();
                DataBaseContext context = asyncScope
                    .ServiceProvider.GetRequiredService<DataBaseContext>();
                var job = await context.VideoProcessJob.Include(job => job.VideoFile)
                    .FirstAsync(processJob => processJob.Id == _job.Id, cancellationToken: stoppingToken);
                job.Status = JobStatus.Running;
                await context.SaveChangesAsync(stoppingToken);
                var id = await SendProcessRequest(job);
                var status = await WaitForProcess(id);
                if (!status)
                {
                    job.Status = JobStatus.Error;
                    await context.SaveChangesAsync(stoppingToken);
                    continue;
                }

                var downloadFile = await DownloadProcessesFile(id,
                    "processed_" + job.VideoFile.Path.Split("/")[1]);
                

                VideoFileService videoFileService = asyncScope
                    .ServiceProvider.GetRequiredService<VideoFileService>();

                await videoFileService.AddVideo(downloadFile, null, job.VideoId, null,true);
                
                job.Status = JobStatus.Success;
                await context.SaveChangesAsync(stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Failed to execute CameraRecordService: {ex.Message}.");
            }
        }
    }

    private async Task<string> SendProcessRequest(VideoProcessJob job)
    {
        HttpClient client = new HttpClient();
        Uri uri = new Uri(settings.VideoProcessor.URL + "/process");
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("url", settings.AWS.Endpoint +"/"+ job.VideoFile.Path),
        });
        var result = await client.PostAsync(uri, formContent);
        return  (await result.Content.ReadAsStringAsync()).Trim();
    }

    private async Task<string> DownloadProcessesFile(string id,string filename)
    {
        _logger.LogInformation($"Downloading  {id} in {filename}");
        HttpClient client = new HttpClient();
        var query = new Dictionary<string, string>()
        {
            ["id"] = id,
        };
        var uri = QueryHelpers.AddQueryString(settings.VideoProcessor.URL + "/file", query!);
        var response = await client.GetAsync(uri);
        var proccessedTempPath = Path.Combine("/tmp", filename);
        await using (var fs = new FileStream(proccessedTempPath,FileMode.CreateNew, FileAccess.ReadWrite))
        {
            await response.Content.CopyToAsync(fs);
        }

        return proccessedTempPath;
    }
    private async Task<bool> WaitForProcess(string id)
    {
        HttpClient client = new HttpClient();
        var query = new Dictionary<string, string>()
        {
            ["id"] = id,
        };

        var uri = QueryHelpers.AddQueryString(settings.VideoProcessor.URL + "/status", query!);
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            await timer.WaitForNextTickAsync())
        {
            var result = await client.GetAsync(uri);
            var status = await result.Content.ReadAsStringAsync();
            status = status.Trim();
            _logger.LogInformation($"Status for {id} is {status}");
            if (status == "done")
            {
                return true;
            }
            if (status == "error")
            {
                return false;
            }
        }

        return false;

    }
}