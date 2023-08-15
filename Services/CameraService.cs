using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MonitoringNetCore.Common.Utils;
using MonitoringNetCore.Domain.Entities;
using MonitoringNetCore.Persistence.Contexts;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Settings = MonitoringNetCore.Common.Settings;

namespace MonitoringNetCore.Services;

public class StreamPath
{
    public string Source { get; set; } 
    public string RunOnInit { get; set; } = "";
    public bool RunOnInitRestart { get; set; } = true;
    public string RunOnReady { get; set; } = "";
    public bool RunOnReadyRestart { get; set; } = true;
}
public class CameraService
{
    private readonly DataBaseContext _context;
    private readonly Settings settings;
    private readonly string YAMLFile = "mediamtx.yml";

    private readonly string SegmentationCmd;
       

    private readonly string Host;
    private readonly string HslUrl;
    private readonly string RtspUrl;
    public string RtspUrlInternal { get; set; }
    private readonly string MjpegCmd;
       

    public CameraService(DataBaseContext context, Settings configuration)
    {
        _context = context;
        this.settings = configuration;
        this.Host = settings.MediaMTX.Host;
        this.HslUrl = String.Format("http://{0}:8888/", settings.MediaMTX.HslHost);
        this.RtspUrl = String.Format("rtsp://{0}:8554/",Host);
        this.RtspUrlInternal = String.Format("rtsp://{0}:8554/",Host);
        this.MjpegCmd =
            "ffmpeg -hide_banner  -use_wallclock_as_timestamps 1 -re -i %url -vcodec h264 -preset ultrafast -f rtsp " +
            RtspUrl + "%streamkey";
        
        this.SegmentationCmd =  "ffmpeg -i rtsp://localhost:$RTSP_PORT/$MTX_PATH -c copy -f segment -strftime 1 -reset_timestamps 1 -segment_time "+
                            settings.MediaMTX.SegmentDuration
                            +" -segment_format "+ settings.MediaMTX.SegmentFormat
                            +" " + settings.MediaMTX.SegmentExtra 
                            +" $MTX_PATH_saved_%Y-%m-%d_%H-%M-%S." + settings.MediaMTX.SegmentExt;

    }



    public async Task RebuildYamlFile()
    {
        var obj = LoadYamlDictionary();
        
        var paths = (Dictionary<object, object>)obj["paths"];
        var cameras = await GetAllCameras();
        var keysToRemove = paths.Keys.ToList();
        keysToRemove.Remove("all");
        foreach (var key in keysToRemove)
        {
            paths.Remove(key);
        }
        foreach (var camera in cameras)
        {
            var autoSaveCmd = "";
            if (camera!.AutoSave)
                autoSaveCmd = SegmentationCmd;
            if (camera!.Type == CameraType.Rtsp || camera!.Type == CameraType.Rtsps ||
                camera!.Type == CameraType.Rtmp || camera!.Type == CameraType.Rtmps )
            {
                paths[camera.StreamKey] = new StreamPath()
                {
                    Source = camera.Url,
                    RunOnReady = autoSaveCmd
                };
            }
            if (camera!.Type == CameraType.Mjpeg)
            {
                paths[camera.StreamKey+"_cmd"] = new StreamPath()
                {
                    
                    RunOnInit = MjpegCmd.Replace("%url",camera.Url).
                        Replace("%streamkey",camera.StreamKey)
                };
                paths[camera.StreamKey] = new StreamPath()
                {
                    Source = "publisher",
                    RunOnReady = autoSaveCmd
                };
            }
            
            if (camera!.IsRealtimeProcess)
            {
                paths[camera.StreamKey+"_process"] = new StreamPath()
                {
                    Source = "publisher",
                    RunOnReady = SegmentationCmd
                };
            }
        }
        
        
        var serializer = GetSerializer();
        await using var writer = new StringWriter();
        serializer.Serialize(writer, obj);
        await File.WriteAllTextAsync(Path.Join(settings.MediaMTX.Path, YAMLFile),writer.ToString());
    }



    public async Task<Camera> AddCamera(Camera camera)
    {
        _context.Add(camera);
        await _context.SaveChangesAsync();
        await RebuildYamlFile();
        return camera;
    }
    public bool CameraExists(int id)
    {
        return _context.Camera.Any(e => e.Id == id);
    }
    public async Task<Camera> UpdateCamera(Camera camera)
    {
        _context.Update(camera);
        await _context.SaveChangesAsync();
        await RebuildYamlFile();
        return camera;
    }

    public async Task DisableAllRealtimeProcesses()
    {

        foreach (var cam in await _context.Camera.ToListAsync())
        {
            cam.IsRealtimeProcess = false;
        }
        await _context.SaveChangesAsync();
        await RebuildYamlFile();
    }
    public async Task SetRealtimeProcessingForCamera(int? id)
    {
        foreach (var cam in await _context.Camera.ToListAsync())
        {
            cam.IsRealtimeProcess = false;
        }

        var camera = await _context.Camera
            .FirstOrDefaultAsync(m => m.Id == id);
        camera.IsRealtimeProcess = true;
        await _context.SaveChangesAsync();

        await RebuildYamlFile();
        
        
        return ;
    }
    
    public async Task<List<Camera>> GetAllCameras()
    {
        return await _context.Camera.ToListAsync();
    }

    public async Task<Camera> GetCamera(int? id)
    {
        // if (id == null)
        // {
        //     return null;
        // }
        return await _context.Camera
            .FirstAsync(m => m.Id == id);
    }
    
    public async Task<Camera?> GetCamera(string? streamKey)
    {
        if (streamKey == null)
        {
            return null;
        }

        return await _context.Camera.FirstOrDefaultAsync(camera => camera.StreamKey.Equals(streamKey));
    }
    public async Task<string> GetLiveStreamForCamera(int? id)
    {
        var camera = await GetCamera(id);
        return new Uri(new Uri(HslUrl), camera.StreamKey).ToString();
    }
    public async Task<string> GetProcessStreamForCamera(int? id)
    {
        var camera = await GetCamera(id);
        return new Uri(new Uri(HslUrl), camera!.StreamKey+"_process").ToString();
    }

    public async Task StartRealtimeProcess(int? id)
    {
        await SetRealtimeProcessingForCamera(id);
        HttpClient client = new HttpClient();
        var urls = await GetRtspUrlForProcessing(id);


        Uri uri = new Uri(settings.VideoProcessor.URL + "/rtps");
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("read_url", urls[0]),
            new KeyValuePair<string, string>("write_url", urls[1])
        });
        var result = await client.PostAsync(uri, formContent);
    }
    public async Task<string[]> GetRtspUrlForProcessing(int? id)
    {
        var camera = await GetCamera(id);
        var readUrl = new Uri(new Uri(RtspUrl), camera!.StreamKey).ToString();
        var writeUrl = new Uri(new Uri(RtspUrl), camera.StreamKey+"_process").ToString();
        return new [] {readUrl,writeUrl} ;
    }

    public async Task StopRealtimeProcessingForAll()
    {
        await DisableAllRealtimeProcesses();

        HttpClient client = new HttpClient();
        Uri uri = new Uri(settings.VideoProcessor.URL + "/rtps");
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("stop", "now"),
        });
        var result = await client.PostAsync(uri, formContent);
    }
    public async Task RemoveCamera(int? id)
    {
        var camera = await _context.Camera.FindAsync(id);
        _context.Camera.Remove(camera);
        await _context.SaveChangesAsync();
        await RebuildYamlFile();
    }

    public string GetYamlContent()
    {   
        var obj = LoadYamlDictionary();
        var serializer = GetSerializer();
        using var writer = new StringWriter();
        serializer.Serialize(writer, obj);

        return writer.ToString();
    }

    private Dictionary<object, object> LoadYamlDictionary()
    {
        var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithTypeConverter(new ObjectYamlTypeConverter()).Build();
        using var reader = new StreamReader(Path.Join(settings.MediaMTX.Path, YAMLFile));
        var obj = deserializer.Deserialize<Dictionary<object, object>>(reader);
        return obj;
    }
    private static ISerializer GetSerializer()
    {
        var serializer = new SerializerBuilder().
            WithQuotingNecessaryStrings(true).
            // WithTypeResolver(new StaticTypeResolver()).
            // WithTypeConverter(new ObjectYamlTypeConverter()).
            WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        return serializer;
    }
}