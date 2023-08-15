using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MonitoringNetCore.Domain.Entities;

public class VideoFile
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [DisplayName("مسیر")] public string Path { get; set; } = String.Empty;
    public string ThumbnailPath { get; set; }= String.Empty;

    [DisplayName("وضعیت پردازش")]
    public bool IsProcessed { get; set; } = false;
    
    [DataType(DataType.Date)]
    [DisplayName("تاریخ آپلود")]
    public DateTime UploadDate { get; set; }
    
    [MaxLength(128), ForeignKey("IdentityUser")]
    public virtual string? UserId { get; set; }
    [DisplayName("کاربر ایجاد کننده")]
    public virtual IdentityUser IdentityUser { get; set;} = null!;

    [ForeignKey("Camera")]
    public virtual int? CameraId { get; set; }
    [DisplayName("دوربین")]
    public virtual Camera? Camera { get; set;}
    
    [ForeignKey("OriginalVideo")]
    public virtual int? OriginalVideoId { get; set; }

    public virtual VideoFile OriginalVideo { get; set;} = null!;

    [InverseProperty(nameof(OriginalVideo))]
    public virtual VideoFile ProcessedVideo { get; set;} = null!;
}