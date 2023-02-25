using System.Reflection;

namespace Journalizing;

internal static class Log
{
    static readonly string assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";

    public static void output(dynamic? log)
    {
        string str = Convert.ToString(log);

        if (string.IsNullOrWhiteSpace(str))
        {
            return;
        }

        Console.WriteLine(str);

        string filePath = @$"{assemblyName}.log";
        System.IO.File.AppendAllText(filePath, $"{str}\n");
    }
}