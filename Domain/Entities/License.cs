using System.ComponentModel.DataAnnotations.Schema;

namespace MonitoringNetCore.Domain.Entities;

public class License
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Value { get; set; } 
}