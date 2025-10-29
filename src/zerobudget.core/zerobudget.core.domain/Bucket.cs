using ECO;
using Resulz;

namespace zerobudget.core.domain;

public sealed partial class Bucket : AggregateRoot<int>
{
    #region  Properties
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal DefaultLimit { get; private set; } = 0;
    public decimal DefaultBalance { get; private set; } = 0;
    public bool Enabled { get; private set; } = true;
    #endregion

    #region Constructors
    private Bucket() : base() { }
    private Bucket(string name, string description, decimal defaultLimit) : base()
        => (Name, Description, DefaultLimit) = (name, description, defaultLimit);
    #endregion

    #region Factory Methods
    public static OperationResult<Bucket> Create(string name, string description, decimal defaultLimit)
        => Validate(name, description, defaultLimit)
            .IfSuccessThenReturn(() => new Bucket(name, description, defaultLimit));
    #endregion

    #region Methods
    public OperationResult Update(string name, string description, decimal defaultLimit)
        => Validate(name, description, defaultLimit, Enabled)
            .IfSuccess(res => (Name, Description, DefaultLimit) = (name, description, defaultLimit));

    public OperationResult UpdateDefaultBalance(decimal defaultBalance)
        => ValidateDefaultBalance(defaultBalance)
            .IfSuccess(res => DefaultBalance = defaultBalance);

    public OperationResult<MonthlyBucket> CreateMonthly(short year, short month)
        => ValidateMonthlyBucketCreation(year, month)
            .IfSuccessThenReturn(() => new MonthlyBucket(year, month, Identity, Description, DefaultLimit));

    public OperationResult Enable()
        => ValidateStatusChange(Enabled, true)
            .IfSuccess(res => Enabled = true);

    public OperationResult Disable()
        => ValidateStatusChange(Enabled, false)
            .IfSuccess(res => Enabled = false);
    #endregion
}
