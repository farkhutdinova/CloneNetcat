namespace CloneNetcat;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowNetcat();
            return;
        }

        if (args.Length < 4)
        {
            Console.WriteLine("Usage: ccnc -l -p <port>");
        }

        var tool = args[0];
        var command = args[1];
        var key1 = args[2];
        var value1 = args[3];
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
}