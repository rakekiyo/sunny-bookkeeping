namespace Journalizing;

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
}