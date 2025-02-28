namespace PlatformService.Dtos
{
  // data to publish to the message bus
  public class PlatformPublishedDto
  {
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Event { get; set; }
  }
}
