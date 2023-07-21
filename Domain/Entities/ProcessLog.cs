using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Monitoring.Site.Domain.Entities;

public class ProcessLog
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Result { get; set; }
    
    public string Status { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime CreationDate { get; set; }
    
    [MaxLength(128), ForeignKey("VideoFile")]
    public virtual int VideoId { get; set; }

    public virtual VideoFile VideoFile { get; set;}
    public string Path { get; set; }

    public ProcessLog()
    {
        this.CreationDate  = DateTime.UtcNow;
    }
}