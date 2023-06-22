using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Site.Domain.Entities;

public class VideoFile
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Path { get; set; }
    [DataType(DataType.Date)]
    public DateTime UploadDate { get; set; }
}