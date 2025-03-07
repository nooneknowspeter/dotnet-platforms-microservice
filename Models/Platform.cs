using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
  // internal representation of data
  public class Platform
  {
    [Key] // primary key; Id
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Cost { get; set; }

  }

}
