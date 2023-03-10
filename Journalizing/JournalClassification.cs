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
            debitAccount => debitAccount,
            debitAccount => journals
                .Where(journal => journal.DebitAccount == debitAccount) // 貸方科目で分類分け
                .Where(journal => !string.IsNullOrWhiteSpace(journal.DebitAccount) || journal.DebitMoney != null)   // 貸方科目か貸方金額どちらかに入力がある
                .Select(journal => journal.CreateDebitJournal()).ToList());

        // 貸方科目別伝票を作成
        Dictionary<string, List<Journal>> journalsByCreditAccount = creditAccounts.ToDictionary(
            creditAccount => creditAccount,
            creditAccount => journals
                .Where(Journal => Journal.CreditAccount == creditAccount) // 借方科目で分類訳
                .Where(journal => !string.IsNullOrWhiteSpace(journal.CreditAccount) && journal.CreditMoney != null) // 借方科目か借方金額どちらかに入力がある
                .Select(journal => journal.CreateCreditJournal()).ToList());

        return (journalsByDebitAccount, journalsByCreditAccount);
    }
}