using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CloneNetcat;

internal sealed class TcpServer
{
    public TcpServer()
    {
        ShowNetcat();
        StartServer();
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

    private void StartServer()
    {
        var port = 8888;
        var hostAddress = IPAddress.Parse("127.0.0.1");
        var listener = new TcpListener(hostAddress, port);
        listener.Start();

        using var client = listener.AcceptTcpClient();

        var buffer = new byte[256];
        var stream = client.GetStream();

        while (stream.Read(buffer, 0, buffer.Length) != 0)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            Console.WriteLine(message);
        }
    }
}