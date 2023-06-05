using DotNetty.Codecs.Mqtt.Packets;
using Microsoft.Azure.Devices.Client;
using System.Text;

namespace WorkerServiceThatSimulatesADevice;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly IConfiguration _configuration;

    private DeviceClient deviceClient;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        deviceClient = 
            DeviceClient.CreateFromConnectionString(
            _configuration.GetConnectionString("Device"),
            TransportType.Mqtt);

        while (!stoppingToken.IsCancellationRequested)
        {
            Message receivedMessage = await deviceClient.ReceiveAsync();
            if (receivedMessage == null)
                continue;

            string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            Console.WriteLine($"Received C2D message: {messageData}");

            // Complete the message to indicate processing is finished
            await deviceClient.CompleteAsync(receivedMessage);
        }
    }


}