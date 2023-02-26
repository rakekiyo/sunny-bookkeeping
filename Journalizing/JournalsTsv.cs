using System.Text;
namespace Journalizing;

/// <summary>
/// 仕分け伝票
/// </summary>
internal static class JournalTsv
{
    public static (string firstLine, IList<Journal>) GetJournalsFromTsv(string path)
    {
        List<Journal> Journals = new();
        int serialNumber = 0;
        int? currentMonth = null;

        string[] lines = System.IO.File.ReadAllLines(path);
        string firstLine = lines[0];

        // 先頭から2行は読み飛ばし
        foreach (var line in System.IO.File.ReadAllLines(path).Skip(2))
        {
            string[] items = line.Split('\t');

            currentMonth = int.TryParse(items[0], out int month) ? month : currentMonth;

            Journals.Add(new Journal()
            {
                SerialNumber = serialNumber++,
                Month = currentMonth,
                Date = int.TryParse(items[1], out int date) ? date : null,
                DebitMoney = decimal.TryParse(items[2].Replace("¥", "").Replace(",", ""), out Decimal debitMoney) ? debitMoney : null,
                DebitAccount = items[3].Trim(),
                Summary = items[4].Trim(),
                CreditAccount = items[5].Trim(),
                CreditMoney = Decimal.TryParse(items[6].Replace("¥", "").Replace(",", ""), out Decimal creditMoney) ? creditMoney : null,
            });
        }

        return (firstLine, Journals);
    }

    public static string GetTsvFromJournalsByAccount(string firstLine, Dictionary<string, List<Journal>> journalsByAccount)
    {
        System.Text.StringBuilder tsv = new();

        tsv.Append(firstLine);
        tsv.AppendLine();

        // 科目ごとにループ
        journalsByAccount
            .OrderBy(keyValue => keyValue.Key)
            .ToList().ForEach(keyValue =>
        {
            tsv.AppendLine(keyValue.Key.IfNullOrWhiteSpace("（未入力）"));
            tsv.AppendLine("月\t日\t摘要\t借方\t貸方");

            int? currentMonth = null;

            foreach (var journal in keyValue.Value.OrderBy(s => s.Month))
            {
                if (currentMonth != journal.Month)
                {
                    if (currentMonth != null)
                    {
                        insertTotals(tsv, (int)currentMonth);
                    }

                    currentMonth = journal.Month;
                }

                tsv.Append($"{journal.Month}\t");
                tsv.Append($"{journal.Date}\t");
                tsv.Append($"{journal.Summary}\t");
                tsv.Append($"{journal.DebitMoney?.ToString("#,#") ?? ""}\t");
                tsv.Append($"{journal.CreditMoney?.ToString("#,#") ?? ""}\t");
                tsv.AppendLine();
            }

            if (currentMonth != null)
            {
                insertTotals(tsv, (int)currentMonth);
            }


            tsv.AppendLine();
        });

        return tsv.ToString();

        void insertTotals(StringBuilder tsv, int month)
        {
            tsv.AppendLine($"\t\t{month}月合計");
            tsv.AppendLine($"\t\t{month}月累計");
            tsv.AppendLine();
            tsv.AppendLine();
            tsv.AppendLine();
        }

    }
}