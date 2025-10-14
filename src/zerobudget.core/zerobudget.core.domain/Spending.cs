using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class Spending : AggregateRoot<int>
{
    #region Properties
    public int BucketId { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public decimal Amount { get; protected set; }
    public string Owner { get; protected set; } = string.Empty;
    public string[] Tags { get; protected set; } = [];
    public bool Enabled { get; private set; } = true;
    #endregion

    #region Constructors
    protected Spending() : base() { }
    protected Spending(string description, decimal amount, string owner, Tag[] tags, int bucketId) : base()
        => (Description, Amount, Owner, Tags, BucketId) = (description, amount, owner, tags.ToTagNames(), bucketId);
    #endregion

    #region Factory Methods
    public static OperationResult<Spending> Create(string description, decimal amount, string owner, Tag[] tags, Bucket bucket)
        => Validate(bucket, description, amount, owner, tags)
            .IfSuccessThenReturn(() => new Spending(description, amount, owner, tags, bucket.Identity));
    #endregion

    #region Methods
    public OperationResult Update(string description, decimal amount, string owner, Tag[] tags)
        => Validate(description, amount, owner, tags, Enabled)
            .IfSuccess(res => (Description, Amount, Owner, Tags) = (description, amount, owner, tags.ToTagNames()));

    public MonthlySpending CreateMonthly(MonthlyBucket monthlyBucket)
        => new(new DateOnly(monthlyBucket.Year, monthlyBucket.Month, 1), Description, Amount, Owner, Tags, monthlyBucket.Identity);

    public void Enable()
        => Enabled = true;

    public void Disable()
        => Enabled = false;

    public override int GetHashCode()
        => HashCode.Combine(Description, Amount, Owner);

    public override string ToString()
        => $"{Description} : {Amount} ({Owner})";

    public override bool Equals(object? obj)
        => obj is Spending other && GetHashCode() == other.GetHashCode();
    #endregion
}