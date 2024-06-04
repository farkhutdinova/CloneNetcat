using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CloneNetcat;

internal sealed class TcpServer
{
    private TcpListener _listener;

    public TcpServer()
    {
        ShowNetcat();
    }

    public void Start(CancellationToken stoppingToken)
    {
        var cmd = Console.ReadLine();
        var result = WaitForCommand(cmd);
        while (result == false && !stoppingToken.IsCancellationRequested)
        {
            cmd = Console.ReadLine();
            result = WaitForCommand(cmd);
        }
    }

    public async Task Echo(CancellationToken stoppingToken)
    {
        using var client = await _listener.AcceptTcpClientAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var buffer = new byte[256];
            var stream = client.GetStream();

            while (await stream.ReadAsync(buffer, stoppingToken) != 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                Console.WriteLine(message);
            }
        }
    }

    private bool WaitForCommand(string cmd)
    {
        var tokens = cmd.Split(' ');
        if (tokens.Length < 4)
        {
            Console.WriteLine("Usage: ccnc -l -p <port>");
            return false;
        }

        var command = tokens[0];
        var action = tokens[1];
        var key1 = tokens[2];
        var value1 = tokens[3];
        if (command == "ccnc" && action == "-l" && key1 == "-p")
        {
            Console.WriteLine($"Start listening on port {value1}...");
            StartServer(int.Parse(value1));
            return true;
        }

        Console.WriteLine($"Unknown command: {cmd}");
        return false;
    }

    private static void ShowNetcat()
    {
        const string netcatString = """

                                    ...   ..  .......  ........  .......      ...      ........
                                    ....  ..  ..          ..     ..          .. ..        ..
                                    .. .. ..  .....       ..     ..         ..   ..       ..
                                    ..  ....  ..          ..     ..        .........      ..
                                    ..   ...  .......     ..     .......  ..       ..     ..

                                    """;
        Console.WriteLine(netcatString);
    }

    private void StartServer(int port)
    {
        var hostAddress = IPAddress.Parse("127.0.0.1");
        _listener = new TcpListener(hostAddress, port);
        _listener.Start();
    }
}