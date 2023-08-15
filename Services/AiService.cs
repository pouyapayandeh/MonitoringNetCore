using System.Net.Http.Headers;
using Amazon.S3;
using Amazon.S3.Transfer;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;

namespace MonitoringNetCore.Services;

public class AiService
{
    private readonly DataBaseContext _context;
    // private readonly ILogger _logger;
    private readonly Settings _settings;
    IAmazonS3 S3Client { get; set; }

    public AiService(DataBaseContext context, IAmazonS3 s3Client,Settings settings)
    {
        _context = context;
        S3Client = s3Client;
        _settings = settings;
        Console.Out.WriteLine("NEW AI PROCESS");
    }

        
    
    public IQueryable<VideoProcessJob> GetJobsQuery()
    {
        return  _context.VideoProcessJob.AsQueryable();
    }
}