namespace MonitoringNetCore.Common;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class AWS
{
    public string Endpoint { get; set; }= string.Empty;
    public string PublicEndpoint { get; set; }= string.Empty;
    public string AccessKeyId { get; set; }= string.Empty;
    public string SecretAccessKey { get; set; }= string.Empty;
}

public class ConnectionStrings
{
    public string PostgresConnection { get; set; }= string.Empty;
}

public class MediaMTX
{
    public string Path { get; set; }= string.Empty;
    public string Host { get; set; }= string.Empty;
    public string HslHost { get; set; }= string.Empty;
    public int SegmentDuration { get; set; }
    
    public string SegmentExt { get; set; }= string.Empty;
    public string SegmentFormat { get; set; }= string.Empty;
    public string SegmentExtra { get; set; }= string.Empty;
}

public class Settings
{
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public MediaMTX MediaMTX { get; set; }= null!;
    public AWS AWS { get; set; }= null!;
    public VideoProcessor VideoProcessor { get; set; }= null!;
}

public class VideoProcessor
{
    public string URL { get; set; }= string.Empty;
}

