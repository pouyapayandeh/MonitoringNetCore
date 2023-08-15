using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore;
using Monitoring.Common;
using MonitoringNetCore.Common;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;

namespace MonitoringNetCore.Services;

public class VideoFileService
{
    private const string VideoBucket = "uploads";
    private const string ThumbBucket = "thumbnails";
    private readonly DataBaseContext _context;
    private readonly Settings _settings;
    private readonly ILogger _logger;
    private readonly IAmazonS3 _S3Client;
    private readonly bool DeleteAfterUpload = true;
    public VideoFileService(DataBaseContext context, Settings settings, ILogger<VideoFileService> logger,
        IAmazonS3 s3Client)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
        _S3Client = s3Client;
    }

    public async Task<VideoFile?> GetVideo(int? id)
    {
        if (id == null)
        {
            return null;
        }

        return await _context.VideoFile.
            Include(file => file.IdentityUser)
            .Include(file => file.Camera)
            .Include(file => file.OriginalVideo)
            .Include(file => file.ProcessedVideo)
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    private async Task<(int exitCode, string? error)> GenerateThumbnail(string videoPath,string thumbnailPath)
    {
        string args = String.Format("-y -i \"{0}\" -vf \"thumbnail\" -frames:v 1 \"{1}\"",videoPath, thumbnailPath);
        var proc = Process.Start("ffmpeg",args);
        ArgumentNullException.ThrowIfNull(proc);
        // string errorOutput = proc.StandardError.ReadToEnd();
        await proc.WaitForExitAsync();
        return (proc.ExitCode, "");
    }
    
    public async Task AddVideo(string path,string? userId,int? videoId,int? cameraId,bool isProccess = false)
    {
        var filename = Path.GetFileName(path);
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        await UploadFile(path,VideoBucket);
        string thumbPath = Path.Combine("/tmp", filenameWithoutExtension + ".png");
        string thumbFileName = Path.GetFileName(thumbPath);
        await GenerateThumbnail(path, thumbPath);
        await UploadFile(thumbPath,ThumbBucket);

        if (DeleteAfterUpload)
        {
            File.Delete(path);
            File.Delete(thumbPath);
        }
        

        var video = new VideoFile
        {
            UploadDate = DateTime.UtcNow,
            UserId = userId, 
            CameraId = cameraId,
            OriginalVideoId = videoId,
            Path = Path.Combine(VideoBucket,filename),
            ThumbnailPath =  Path.Combine(ThumbBucket, thumbFileName),
            IsProcessed = isProccess
            
        };
        _context.Add(video);
        await _context.SaveChangesAsync();
    }

    public async Task AddVideo(string path, string? userId)
    {
        await AddVideo(path, userId, null, null);
    }

    public async Task<List<VideoFile>> GetVideos()
    {
        return await _context.VideoFile.ToListAsync();
    }
    
    public IQueryable<VideoFile> GetVideosQuery()
    {
        return  _context.VideoFile.AsQueryable();
    }
    private async Task UploadFile(string path,string bucket, string filename)
    {
        await using var newMemoryStream = System.IO.File.OpenRead(path);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = filename,
            BucketName = bucket,
            CannedACL = S3CannedACL.PublicRead,
        };

        var fileTransferUtility = new TransferUtility(_S3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);
    }
    private async Task UploadFile(string path,string bucket)
    {
        await using var inputStream = System.IO.File.OpenRead(path);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = inputStream,
            Key = Path.GetFileName(path),
            BucketName = bucket,
            CannedACL = S3CannedACL.PublicRead,
        };

        var fileTransferUtility = new TransferUtility(_S3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);
    }
}