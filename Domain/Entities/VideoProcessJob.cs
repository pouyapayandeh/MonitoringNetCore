using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringNetCore.Domain.Entities;

public enum JobStatus
{
    Waiting,
    Running,
    Error,
    Success
}
public class VideoProcessJob
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayName("زمان ایجاد")]
    public DateTime CreationDate { get; set; }
    
    [Column(TypeName = "varchar(24)")]
    [DisplayName("وضعیت")]
    public JobStatus Status { get; set; }

    
    [MaxLength(128), ForeignKey("VideoFile")]
    public virtual int VideoId { get; set; }
    
    [DisplayName("فایل ویديو")]
    public virtual VideoFile VideoFile { get; set;} = null!;

    public VideoProcessJob()
    {
        this.CreationDate  = DateTime.UtcNow;
    }
}