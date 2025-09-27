using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class MonthlySpending : AggregateRoot<int>
{
    #region Properties
    public DateOnly Date { get; protected set; }
    public int MonthlyBucketId { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public decimal Amount { get; protected set; }
    public string Owner { get; protected set; } = string.Empty;
    public string[] Tags { get; protected set; } = [];
    #endregion

    #region Constructors
    protected MonthlySpending() : base() { }
    internal MonthlySpending(DateOnly date, string description, decimal amount, string owner, string[] tags, int monthlyBucketId) : base()
        => (Date, Description, Amount, Owner, Tags, MonthlyBucketId) = (date, description, amount, owner, tags, monthlyBucketId);
    #endregion

    #region Factory Methods
    public static OperationResult<MonthlySpending> Create(DateOnly date, string description, decimal amount, string owner, string[] tags, MonthlyBucket monthlyBucket)
        => Validate(date, description, amount, owner, tags)
            .IfSuccessThenReturn(() => new MonthlySpending(date, description, amount, owner, tags, monthlyBucket.Identity));
    #endregion

    #region Methods    
    public void Update(DateOnly date, string description, decimal amount, string owner, string[] tags)
        => Validate(date, description, amount, owner, tags)
            .IfSuccess(res => (Date, Description, Amount, Owner, Tags) = (date, description, amount, owner, tags));

    public override int GetHashCode()
        => HashCode.Combine(Date, Description, Amount, Owner);
    #endregion
}
