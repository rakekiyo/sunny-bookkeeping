using System.Transactions;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

#if DEBUG 
string[] commands = new String[] { "", @"C:\Users\taketsugu.kyohei\Desktop\input.tsv" };
#else
//コマンドライン引数を配列で取得する
string[] commands = System.Environment.GetCommandLineArgs();
#endif


if (commands.Length <= 1)
{
    Console.WriteLine("ドロップされたファイルはありませんでした");
    return;
}

string targetPath = commands[1];

Console.WriteLine("次のファイルがドロップされました");
Console.WriteLine(targetPath);

IList<Journal> Journals = Journal.GetFromTsv(targetPath);

AggrigationWorker.Aggrigate(Journals);


