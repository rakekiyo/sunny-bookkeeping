using System.Transactions;
using System.Globalization;
using System.Text;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;
internal static class JournalClassification
{
    public static void Classify(IList<Journal> _Journals)
    {
        ReadOnlyCollection<Journal> Journals = new(_Journals);

        IList<string> debitAccounts = Journals.Select(s => s.DebitAccount).Distinct().ToList();
        IList<string> creditAccounts = Journals.Select(s => s.CreditAccount).Distinct().ToList();

        // 借方科目別伝票を作成
        Dictionary<string, List<Journal>> aggrigatedDebitJournals = debitAccounts.ToDictionary(
            debitAccount => debitAccount, debitAccount => Journals.Where(Journal => Journal.DebitAccount == debitAccount).ToList());

        // 貸方科目別伝票を作成
        Dictionary<string, List<Journal>> aggrigatedCreditJournals = creditAccounts.ToDictionary(
            creditAccount => creditAccount, creditAccount => Journals.Where(Journal => Journal.CreditAccount == creditAccount).ToList());

        printTsv(aggrigatedDebitJournals, "貸方科目", true);
        printTsv(aggrigatedCreditJournals, "借方科目", false);
    }

    private static void printTsv(Dictionary<string, List<Journal>> aggrigatedJournals, string title, bool isDebit)
    {
        StringBuilder tsv = new StringBuilder(title);

        // 科目ごとにループ
        aggrigatedJournals.ToList().ForEach(Journals =>
        {
            int currentMonth = 0;
            Decimal debitTotal = 0m;
            Decimal creditTotal = 0m;
            Decimal debitAllTotal = 0m;
            Decimal creditAllTotal = 0m;

            tsv.AppendLine();
            tsv.AppendLine(Journals.Key);
            tsv.AppendLine("月\t日\t摘要\t借方\t貸方");

            foreach (var Journal in Journals.Value.OrderBy(s => s.Month))
            {
                if (currentMonth != Journal.Month)
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

                    currentMonth = Journal.Month;
                }

                tsv.Append($"{Journal.Month}\t");
                tsv.Append($"{Journal.Date}\t");
                tsv.Append($"{Journal.Summary}\t");
                tsv.Append($"{(isDebit ? Journal.DebitMoney : "")}\t");
                tsv.Append($"{(isDebit ? "" : Journal.CreditMoney)}\t");
                tsv.AppendLine();

                debitTotal += Journal.DebitMoney;
                creditTotal += Journal.CreditMoney;
            }

            debitAllTotal += debitTotal;
            creditAllTotal += creditTotal;

            tsv.AppendLine($"\t\t合計\t{(isDebit ? "" : debitTotal)}\t{(isDebit ? creditTotal : "")}");
            tsv.AppendLine($"\t\t累計\t{(isDebit ? "" : debitAllTotal)}\t{(isDebit ? creditAllTotal : "")}");
        });

        System.IO.File.WriteAllText($"{title}.tsv", tsv.ToString());
    }
}