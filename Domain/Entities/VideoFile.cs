using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Monitoring.Site.Domain.Entities;

public class VideoFile
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Path { get; set; }

    public string ThumbnailPath { get; set; }

    [DataType(DataType.Date)]
    public DateTime UploadDate { get; set; }
    
    [MaxLength(128), ForeignKey("IdentityUser")]
    public virtual string UserId { get; set; }

    public virtual IdentityUser IdentityUser { get; set;}

}