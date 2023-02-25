/// <summary>
/// 仕分け伝票
/// </summary>
internal struct Journal
{
    public int SerialNumber { get; init; }
    public int Month { get; init; }
    public int Date { get; init; }
    /// <summary>
    /// 借方金額
    /// </summary>
    public decimal DebitMoney { get; init; }
    /// <summary>
    /// 借方科目
    /// </summary>
    public string DebitAccount { get; init; }
    /// <summary>
    /// 摘要
    /// </summary>
    public string Summary { get; init; }
    /// <summary>
    /// 貸方科目
    /// </summary>
    public string CreditAccount { get; init; }
    /// <summary>
    /// 貸型金額
    /// </summary>
    public decimal CreditMoney { get; init; }

    public static IList<Journal> GetFromTsv(string path)
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
}