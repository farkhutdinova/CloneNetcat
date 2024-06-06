using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CloneNetcat;

internal sealed class ListeningServer
{
    private TcpListener _listener;
    private UdpClient _updListener;

    public ListeningServer()
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
        if (_updListener != null)
        {
            await EchoUdp(stoppingToken);
        }
        else if (_listener != null)
        {
            await EchoTcp(stoppingToken);
        }

    }

    private async Task EchoUdp(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Creates an IPEndPoint to record the IP Address and port number of the sender.
                // The IPEndPoint will allow you to read datagrams sent from any source.
                var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = _updListener.Receive(ref remoteIpEndPoint);

                    Console.WriteLine(Encoding.ASCII.GetString(receiveBytes));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // var result = await _updListener.ReceiveAsync(stoppingToken);
                // var bytes = result.Buffer;
                // Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task EchoTcp(CancellationToken stoppingToken)
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
        if (command == "ccnc" && action == "-l")
        {
            var listenerParams = ListenerParams.Parse(tokens[2..]);
            Console.WriteLine($"Start listening on port {listenerParams.Port}...");
            StartServer(listenerParams);
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

    private void StartServer(ListenerParams listenerParams)
    {
        var hostAddress = IPAddress.Parse("127.0.0.1");
        if (listenerParams.IsUdp)
        {
            _updListener = new UdpClient(listenerParams.Port);
            _updListener.Connect(hostAddress, listenerParams.Port);
        }
        else
        {
            _listener = new TcpListener(hostAddress, listenerParams.Port);
            _listener.Start();
        }
    }
}

internal record ListenerParams(bool IsUdp, int Port)
{
    public static ListenerParams Parse(params string[] args)
    {
        var isUdp = false;
        var port = 8080;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-u":
                    isUdp = true;
                    break;
                case "-p":
                    port = int.Parse(args[i + 1]);
                    break;
            }
        }

        return new ListenerParams(isUdp, port);
    }
}