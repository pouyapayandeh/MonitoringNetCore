using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringNetCore.Domain.Entities;

public class ProcessLog
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Result { get; set; } = null!;

    public string Status { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime CreationDate { get; set; }
    
    [MaxLength(128), ForeignKey("VideoFile")]
    public virtual int VideoId { get; set; }

    public virtual VideoFile VideoFile { get; set;} = null!;
    public string Path { get; set; } = null!;

    public ProcessLog()
    {
        this.CreationDate  = DateTime.UtcNow;
    }
}