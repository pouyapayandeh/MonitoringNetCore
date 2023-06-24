using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitoring.Site.Domain.Entities;

public class Camera
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    [DataType(DataType.Date)]
    public DateTime InsertedAt { get; set; }
    public int Type { get; set; }
    public string Url { get; set; }
}