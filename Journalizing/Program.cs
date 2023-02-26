using Journalizing;

string now = DateTime.Now.ToString("yyyyMMddHHmmss");

string inputJournalsPath = @"仕訳帳.txt";
string debitJournalsPath = @$"借方仕分帳_{now}.txt";
string creditJournalsPath = @$"貸方仕分帳_{now}.txt";

#if DEBUG 
inputJournalsPath = @"TestResource\仕訳帳.txt";
debitJournalsPath = @"TestResource\借方仕分帳.txt";
creditJournalsPath = @"TestResource\貸方仕分帳.txt";
#endif

try
{
    // 仕訳リストを取得
    (string firstLine, IList<Journal> journals)
        = JournalTsv.GetJournalsFromTsv(inputJournalsPath);

    // 分類訳
    (Dictionary<string, List<Journal>> journalsByDebitAccount,
        Dictionary<string, List<Journal>> journalsByCreditAccount)
        = JournalClassification.ClassifyByDebitAndCreditAccounts(journals);

    // TSV作成
    string journalsByDebitAccountTsv
        = JournalTsv.GetTsvFromJournalsByAccount(firstLine, journalsByDebitAccount);
    string journalsByCreditAccountTsv
        = JournalTsv.GetTsvFromJournalsByAccount(firstLine, journalsByCreditAccount);

    // ファイル出力
    System.IO.File.WriteAllText(debitJournalsPath, journalsByDebitAccountTsv);
    System.IO.File.WriteAllText(creditJournalsPath, journalsByCreditAccountTsv);
}
catch (Exception ex)
{
    Log.output(ex.Message);
    Log.output(ex.StackTrace);
    System.Environment.Exit(1);
}