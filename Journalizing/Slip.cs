internal struct Slip
{
    public int SerialNumber { get; init; }
    // public int Year { get; init; }
    public int Month { get; init; }
    public int Date { get; init; }
    public decimal DebitMoney { get; init; }
    /// <summary>
    /// 借方科目
    /// </summary>
    public string DebitAccount { get; init; }
    public string Summary { get; init; }
    /// <summary>
    /// 貸方科目
    /// </summary>
    public string CreditAccount { get; init; }
    public decimal CreditMoney { get; init; }

    public static IList<Slip> GetFromTsv(string path)
    {
        List<Slip> slips = new();
        int serialNumber = 0;

        // 1行目は読み飛ばし
        foreach (var line in System.IO.File.ReadAllLines(path).Skip(1))
        {
            string[] items = line.Split('\t');

            slips.Add(new Slip()
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

        return slips;
    }
}