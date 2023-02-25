namespace Journalizing;

/// <summary>
/// 仕分け伝票
/// </summary>
internal static class JournalTsv
{
    public static IList<Journal> GetJournalsFromTsv(string path)
    {
        List<Journal> Journals = new();
        int serialNumber = 0;

        // 1行目は読み飛ばし
        foreach (var line in System.IO.File.ReadAllLines(path).Skip(1))
        {
            string[] items = line.Split('\t');

            Journals.Add(new Journal()
            {
                SerialNumber = serialNumber++,
                Month = Convert.ToInt32(items[0]),
                Date = Convert.ToInt32(items[1]),
                DebitMoney = Decimal.TryParse(items[2].Replace("¥", "").Replace(",", ""), out Decimal tryDebitMoney) ? tryDebitMoney : 0,
                DebitAccount = items[3],
                Summary = items[4],
                CreditAccount = items[5],
                CreditMoney = Decimal.TryParse(items[6].Replace("¥", "").Replace(",", ""), out Decimal tryCreditMoney) ? tryCreditMoney : 0,
            });
        }

        return Journals;
    }

    public static string GetTsvFromJournalsByAccount(Dictionary<string, List<Journal>> journalsByAccount)
    {
        System.Text.StringBuilder tsv = new();

        // 科目ごとにループ
        journalsByAccount.ToList().ForEach(journals =>
        {
            tsv.AppendLine(journals.Key);
            tsv.AppendLine("月\t日\t摘要\t借方\t貸方");

            foreach (var Journal in journals.Value.OrderBy(s => s.Month))
            {
                tsv.Append($"{Journal.Month}\t");
                tsv.Append($"{Journal.Date}\t");
                tsv.Append($"{Journal.Summary}\t");
                tsv.Append($"{Journal.DebitMoney.ToString("#,#")}\t");
                tsv.Append($"{Journal.CreditMoney.ToString("#,#")}\t");
                tsv.AppendLine();
            }

            tsv.AppendLine();
        });

        return tsv.ToString();
    }
}