using ECO;
using Resulz;

namespace zerobudget.core.domain;

public sealed partial class MonthlySpending : AggregateRoot<int>, IEquatable<MonthlySpending>
{
    #region Properties
    public DateOnly Date { get; private set; }
    public int MonthlyBucketId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public string[] Tags { get; private set; } = [];
    #endregion

    #region Constructors
    private MonthlySpending() : base() { }
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

    public override bool Equals(object? obj)
        => obj is MonthlySpending other && Equals(other);

    public bool Equals(MonthlySpending? obj)
    {
        if (obj is null) return false;
        return Date == obj.Date &&
               Description == obj.Description &&
               Amount == obj.Amount &&
               Owner == obj.Owner;
    }


    #endregion
}
