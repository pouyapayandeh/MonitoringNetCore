using System.Net.Http.Headers;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Common;
using Monitoring.Presistence.Contexts;
using Monitoring.Site.Domain.Entities;

namespace Monitoring.Site.Services;

public class AiProcessor
{
    private readonly DataBaseContext _context;
    // private readonly ILogger _logger;
    private readonly Settings _settings;
    IAmazonS3 S3Client { get; set; }

    public AiProcessor(DataBaseContext context, IAmazonS3 s3Client,Settings settings)
    {
        _context = context;
        S3Client = s3Client;
        _settings = settings;
        Console.Out.WriteLine("NEW AI PROCESS");
    }

    public async Task ProcessWithVideoOutput(VideoFile videoFile)
    {
        var processLog = new ProcessLog
        {
            Status = "created",
            VideoId = videoFile.Id,
            Path = Path.Combine("processed","processed"+videoFile.Path.Split("/")[1]),
            Result = ""
        };
        var process = _context.Add(processLog);
        await _context.SaveChangesAsync();
        var tempPath = Path.Combine("/tmp", "prep" + videoFile.Path.Split("/")[1]);
        var downloadRequest = new TransferUtilityDownloadRequest
        {
            
            Key =  videoFile.Path.Split("/")[1],
            BucketName = videoFile.Path.Split("/")[0],
            FilePath = tempPath

        };
        processLog.Status = "Downloading from S3";
        await _context.SaveChangesAsync();
        var fileTransferUtility = new TransferUtility(S3Client);
        await fileTransferUtility.DownloadAsync(downloadRequest);
        
        
        HttpClient client = new HttpClient();
        Uri uri = new Uri(_settings.VideoProcessorServiceURL + "/video");
        using (var stream = new FileStream(tempPath,FileMode.Open, FileAccess.Read))
        {
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("*/*");

            var requestContent = new MultipartFormDataContent(); 
            //    here you can specify boundary if you need---^

            requestContent.Add(content, "video", "prep" + videoFile.Path.Split("/")[1]);
            processLog.Status = "Uploading To AI Engine";
            await _context.SaveChangesAsync();        
            //Send it
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,uri);
            request.Content = requestContent;
            //exception thrown here... 
            
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            
            processLog.Status = "Downloading from AI Engine";
            await _context.SaveChangesAsync();   
            var proccessedTempPath = Path.Combine("/tmp", "processed" + videoFile.Path.Split("/")[1]);
            
            await using (var fs = new FileStream(proccessedTempPath,FileMode.Open, FileAccess.ReadWrite))
            {
                await response.Content.CopyToAsync(fs);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = fs,
                    Key =  "processed"+videoFile.Path.Split("/")[1],
                    BucketName = "processed",
                    CannedACL = S3CannedACL.PublicRead,
                        
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            processLog.Status = "Finished";
            processLog.Result = "Success";
            await _context.SaveChangesAsync();
        }
        return;
        
        
    }
}