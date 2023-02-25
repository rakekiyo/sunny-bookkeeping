using System.Transactions;
using System.Globalization;
using System.Text;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;
internal static class AggrigationWorker
{
    public static void Aggrigate(IList<Slip> _slips)
    {
        ReadOnlyCollection<Slip> slips = new(_slips);

        IList<string> debitAccounts = slips.Select(s => s.DebitAccount).Distinct().ToList();
        IList<string> creditAccounts = slips.Select(s => s.CreditAccount).Distinct().ToList();

        // 借方科目別伝票を作成
        Dictionary<string, List<Slip>> aggrigatedDebitSlips = debitAccounts.ToDictionary(
            debitAccount => debitAccount, debitAccount => slips.Where(slip => slip.DebitAccount == debitAccount).ToList());

        // 貸方科目別伝票を作成
        Dictionary<string, List<Slip>> aggrigatedCreditSlips = creditAccounts.ToDictionary(
            creditAccount => creditAccount, creditAccount => slips.Where(slip => slip.CreditAccount == creditAccount).ToList());

        printTsv(aggrigatedDebitSlips, "貸方科目", true);
        printTsv(aggrigatedCreditSlips, "借方科目", false);
    }

    private static void printTsv(Dictionary<string, List<Slip>> aggrigatedSlips, string title, bool isDebit)
    {
        StringBuilder tsv = new StringBuilder(title);

        // 科目ごとにループ
        aggrigatedSlips.ToList().ForEach(slips =>
        {
            int currentMonth = 0;
            Decimal debitTotal = 0m;
            Decimal creditTotal = 0m;
            Decimal debitAllTotal = 0m;
            Decimal creditAllTotal = 0m;

            tsv.AppendLine();
            tsv.AppendLine(slips.Key);
            tsv.AppendLine("月\t日\t摘要\t借方\t貸方");

            foreach (var slip in slips.Value.OrderBy(s => s.Month))
            {
                if (currentMonth != slip.Month)
                {
                    if (currentMonth > 0)
                    {
                        debitAllTotal += debitTotal;
                        creditAllTotal += creditTotal;

                        tsv.AppendLine($"\t\t合計\t{(isDebit ? "" : debitTotal)}\t{(isDebit ? creditTotal : "")}");
                        tsv.AppendLine($"\t\t累計\t{(isDebit ? "" : debitAllTotal)}\t{(isDebit ? creditAllTotal : "")}");
                    }

                    debitTotal = 0m;
                    creditTotal = 0m;

                    currentMonth = slip.Month;
                }

                tsv.Append($"{slip.Month}\t");
                tsv.Append($"{slip.Date}\t");
                tsv.Append($"{slip.Summary}\t");
                tsv.Append($"{(isDebit ? slip.DebitMoney : "")}\t");
                tsv.Append($"{(isDebit ? "" : slip.CreditMoney)}\t");
                tsv.AppendLine();

                debitTotal += slip.DebitMoney;
                creditTotal += slip.CreditMoney;
            }

            debitAllTotal += debitTotal;
            creditAllTotal += creditTotal;

            tsv.AppendLine($"\t\t合計\t{(isDebit ? "" : debitTotal)}\t{(isDebit ? creditTotal : "")}");
            tsv.AppendLine($"\t\t累計\t{(isDebit ? "" : debitAllTotal)}\t{(isDebit ? creditAllTotal : "")}");
        });

        System.IO.File.WriteAllText($"{title}.tsv", tsv.ToString());
    }
}