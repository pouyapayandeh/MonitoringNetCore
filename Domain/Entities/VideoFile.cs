using System.ComponentModel.DataAnnotations;
namespace Monitoring.Site.Domain.Entities;

public class VideoFile
{

    public int Id { get; set; }
    public string Path { get; set; }
    [DataType(DataType.Date)]
    public DateTime UploadDate { get; set; }
}