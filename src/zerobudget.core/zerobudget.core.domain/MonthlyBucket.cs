using ECO;


namespace zerobudget.core.domain;

public partial class MonthlyBucket : AggregateRoot<int>
{
    #region Properties
    public short Year { get; protected set; } = 0;
    public short Month { get; protected set; } = 0;
    public decimal Balance { get; protected set; } = 0;
    public Bucket Bucket { get; protected set; } = null!;
    public string Description { get; protected set; } = string.Empty;
    public decimal Limit { get; protected set; } = 0;

    #endregion

    #region Methods
    #endregion

    #region Constructors
    protected MonthlyBucket() : base() { }
    internal MonthlyBucket(short year, short month, Bucket bucket) : base()
    {
        Year = year;
        Month = month;
        Bucket = bucket;
        Description = bucket.Description;
        Limit = bucket.DefaultLimit;
    }
    #endregion

    #region Methods
    public void UpdateBalance(decimal balance)
        => Balance = balance;
    #endregion
}
