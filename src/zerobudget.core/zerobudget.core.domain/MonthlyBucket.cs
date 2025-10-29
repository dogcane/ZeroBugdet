using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class MonthlyBucket : AggregateRoot<int>
{
    #region Properties
    public short Year { get; protected set; } = 0;
    public short Month { get; protected set; } = 0;
    public decimal Balance { get; protected set; } = 0;
    public int BucketId { get; protected set; } = 0;
    public string Description { get; protected set; } = string.Empty;
    public decimal Limit { get; protected set; } = 0;
    #endregion

    #region Constructors
    protected MonthlyBucket() : base() { }
    internal MonthlyBucket(short year, short month, int bucketId, string description, decimal defaultLimit) : base()
    {
        Year = year;
        Month = month;
        BucketId = bucketId;
        Description = description;
        Limit = defaultLimit;
    }
    #endregion

    #region Methods
    public OperationResult UpdateBalance(decimal balance)
        => ValidateBalance(balance)
            .IfSuccess(res => Balance = balance);
    #endregion
}
