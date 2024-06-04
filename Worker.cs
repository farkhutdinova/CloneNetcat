namespace CloneNetcat;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var server = new TcpServer();
        server.Start(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Echoing the port...");
            await server.Echo(stoppingToken);
        }
    }
}
