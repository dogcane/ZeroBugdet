using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class Spending : AggregateRoot<int>
{
    #region Properties
    public DateOnly Date { get; protected set; }
    public int BucketId { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public decimal Amount { get; protected set; }
    public string Owner { get; protected set; } = string.Empty;
    public Tag[] Tags { get; protected set; } = Array.Empty<Tag>();
    #endregion

    #region Constructors
    protected Spending() : base() { }
    protected Spending(DateOnly date, string description, decimal amount, string owner, Tag[] tags, int bucketId) : base()
        => (Date, Description, Amount, Owner, Tags, BucketId) = (date, description, amount, owner, tags, bucketId);
    #endregion

    #region Factory Methods
    public static OperationResult<Spending> Create(DateOnly date, string description, decimal amount, string owner, Tag[] tags, Bucket bucket)
        => Validate(date, description, amount, owner, tags)
            .IfSuccessThenReturn(() => new Spending(date, description, amount, owner, tags, bucket.Identity));
    #endregion

    #region Methods
    public OperationResult Update(DateOnly date, string description, decimal amount, string owner, Tag[] tags)
        => Validate(date, description, amount, owner, tags)
            .IfSuccess(res => (Date, Description, Amount, Owner, Tags) = (date, description, amount, owner, tags));
    public OperationResult<MonthlySpending> CreateMonthly(MonthlyBucket monthlyBucket)
        => MonthlySpending.Create(Date, Description, Amount, Owner, Tags, monthlyBucket);
    public override int GetHashCode() =>
        HashCode.Combine(Date, Description, Amount, Owner);
    #endregion
}