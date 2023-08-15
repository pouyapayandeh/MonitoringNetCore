using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringNetCore.Domain.Entities;

public enum CameraType
{
    Rtsp,
    Rtsps,
    Rtmp,
    Rtmps,
    Mjpeg,
    
}
public class Camera
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [DisplayName("اسم")]
    public string Name { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayName("تاریخ ایجاد")]
    public DateTime InsertedAt { get; set; }
    
    [Column(TypeName = "varchar(24)")]
    [DisplayName("نوع دوربین")]
    public CameraType Type { get; set; }
    
    [DisplayName("آدرس")]
    public string Url { get; set; }

    [DisplayName("ذخیره‌سازی خودکار")] 
    public bool AutoSave { get; set; } = false;
    
    [DisplayName("فعال")] 
    public bool Enable { get; set; } = true;
    
    [DisplayName("پردازش فعال")] 
    public bool IsRealtimeProcess { get; set; } = false;
    
    [DisplayName("کلید استریم")]
    public string? StreamKey { get; set; }
    public Camera()
    {
        this.InsertedAt  = DateTime.UtcNow;
        this.StreamKey = Guid.NewGuid().ToString("n").Substring(0, 8);
    }
}