namespace PlatformService.Dtos
{
  // external representation of data
  // data transfer object to provide abstraction between end user for security
  public class PlatformReadDto
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Publisher { get; set; }

    public string Cost { get; set; }
  }
}

