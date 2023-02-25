using System.Transactions;
using System.Globalization;
using System.Text;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;

namespace Journalizing;

internal static class JournalClassification
{
    public static (Dictionary<string, List<Journal>>, Dictionary<string, List<Journal>>)
        ClassifyByDebitAndCreditAccounts(IList<Journal> _journals)
    {
        ReadOnlyCollection<Journal> journals = new(_journals);

        IList<string> debitAccounts = journals.Select(s => s.DebitAccount).Distinct().ToList();
        IList<string> creditAccounts = journals.Select(s => s.CreditAccount).Distinct().ToList();

        // 借方科目別伝票を作成
        Dictionary<string, List<Journal>> journalsByDebitAccount = debitAccounts.ToDictionary(
            debitAccount => debitAccount, debitAccount => journals.Where(Journal => Journal.DebitAccount == debitAccount).ToList());

        // 貸方科目別伝票を作成
        Dictionary<string, List<Journal>> journalsByCreditAccount = creditAccounts.ToDictionary(
            creditAccount => creditAccount, creditAccount => journals.Where(Journal => Journal.CreditAccount == creditAccount).ToList());

        return (journalsByDebitAccount, journalsByCreditAccount);
    }
}