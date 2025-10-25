using ECO;
using Resulz;

namespace zerobudget.core.domain;

public sealed partial class Spending : AggregateRoot<int>, IEquatable<Spending>
{
    #region Properties
    public int BucketId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public string[] Tags { get; private set; } = [];
    public bool Enabled { get; private set; } = true;
    #endregion

    #region Constructors
    private Spending() : base() { }
    private Spending(string description, decimal amount, string owner, Tag[] tags, int bucketId) : base()
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

    public OperationResult<MonthlySpending> CreateMonthly(MonthlyBucket monthlyBucket)
        => ValidateMonthlySpendingCreation(monthlyBucket)
            .IfSuccessThenReturn(() => new MonthlySpending(new DateOnly(monthlyBucket.Year, monthlyBucket.Month, 1), Description, Amount, Owner, Tags, monthlyBucket.Identity));

    public OperationResult Enable()
        => ValidateStatusChange(Enabled, true)
            .IfSuccess(res => Enabled = true);

    public OperationResult Disable()
        => ValidateStatusChange(Enabled, false)
            .IfSuccess(res => Enabled = false);

    public override int GetHashCode()
        => HashCode.Combine(Description, Amount, Owner);

    public override string ToString()
        => $"{Description} : {Amount} ({Owner})";

    public override bool Equals(object? obj)
        => obj is Spending other && Equals(other);

    public bool Equals(Spending? other)
    {
        if (other is null) return false;
        return Description == other.Description &&
               Amount == other.Amount &&
               Owner == other.Owner;
    }
    #endregion
}
