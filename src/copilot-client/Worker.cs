using copilot_client_services.interfaces.services;

namespace copilot_client;

public class Worker(ILogger<Worker> logger, IMyCopilotClientService myCopilotClientService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await myCopilotClientService.StartConversation(stoppingToken); 
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
