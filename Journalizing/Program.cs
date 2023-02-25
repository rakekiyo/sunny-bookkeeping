using Journalizing;

string inputJournalsPath = @"仕訳帳.tsv";
string debitJournalsPath = @"借方仕分帳.tsv";
string creditJournalsPath = @"貸方仕分帳.tsv";

#if DEBUG 
inputJournalsPath = @"TestResource\仕訳帳.tsv";
debitJournalsPath = @"TestResource\借方仕分帳.tsv";
creditJournalsPath = @"TestResource\貸方仕分帳.tsv";
#endif

try
{
    // 仕訳リストを取得
    IList<Journal> journals = JournalTsv.GetJournalsFromTsv(inputJournalsPath);

    // 分類訳
    (Dictionary<string, List<Journal>> journalsByDebitAccount,
        Dictionary<string, List<Journal>> journalsByCreditAccount)
        = JournalClassification.ClassifyByDebitAndCreditAccounts(journals);

    // TSV作成
    string journalsByDebitAccountTsv
        = JournalTsv.GetTsvFromJournalsByAccount(journalsByDebitAccount);
    string journalsByCreditAccountTsv
        = JournalTsv.GetTsvFromJournalsByAccount(journalsByCreditAccount);

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