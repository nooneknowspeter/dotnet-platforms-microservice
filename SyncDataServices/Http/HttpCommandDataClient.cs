using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PlatformService.Dtos;

namespace PlatformService.SyncDataService.Http
{
  public class HttpCommandDataClient : ICommandDataClient
  {

    private readonly HttpClient _httpClient;

    // httpClient injected into constructor when called
    public HttpCommandDataClient(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    // implement async call to bus
    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {

      // information to post to CommandsService
      var httpContent = new StringContent(
                      // serialize PlatformReadDto
                      JsonSerializer.Serialize(plat),
                      Encoding.UTF8,
                      // media type
                      "application/json");


      var response = await _httpClient.PostAsync("http://localhost:5108/api/commands/platforms", httpContent);

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("Sync POST to Command Service Successful");
      }
      else
      {
        Console.WriteLine("Sync POST to Command Service Unsuccessful");
      }
    }

  }
}
