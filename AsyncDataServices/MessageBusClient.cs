using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        public async void PublishNewPlatform(
            PlatformPublishedDto platformPublishedDto,
            IConfiguration configuration
        )
        {
            IConfiguration _configuration = configuration;

            var rabbitMQHost = _configuration["RabbitMQHost"];
            var rabbitMQPort = _configuration["RabbitMQPort"];

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = rabbitMQHost;
            factory.Port = int.Parse(rabbitMQPort);

            try
            {
                IConnection connection = await factory.CreateConnectionAsync();
                IChannel channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync("trigger", ExchangeType.Fanout);

                connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("S --> Connected to message bus");

                if (!connection.IsOpen)
                    throw new Exception(
                        "X --> RabbitMQ Connection is Closed, Failed to Send Message"
                    );

                // serialize and send message to bus when platform is created
                var message = JsonSerializer.Serialize(platformPublishedDto);
                Console.WriteLine("S --> RabbitMQ Connection is Open, Sending Message");
                SendMessage(message, channel);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"X --> Failed to connect to message bus: {exception}");
            }
        }

        private async void SendMessage(string message, IChannel channel)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync("trigger", "", body);

                Console.WriteLine("S --> Message Sent");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"X --> Failed to Send Message to Message Bus: {exception}");
            }
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("X --> RabbitMQ Connection Shutdown");
        }
    }
}
