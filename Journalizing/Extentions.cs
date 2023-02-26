namespace Journalizing;


internal static class Extentions
{
    public static string IfNullOrWhiteSpace(this string str, string nextStr)
    {
        return string.IsNullOrWhiteSpace(str) ? nextStr : str;
    }
}