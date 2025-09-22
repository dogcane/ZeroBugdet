using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class Bucket : AggregateRoot<int>
{
    #region Fields
    private readonly List<MonthlySpending> _spendings = [];
    #endregion
    #region  Properties
    public string Name { get; protected set; } = string.Empty;
    public string Description { get; protected set; } = string.Empty;
    public decimal DefaultLimit { get; protected set; } = 0;
    public decimal DefaultBalance { get; protected set; } = 0;
    #endregion

    #region Constructors
    protected Bucket() : base() { }
    protected Bucket(string name, string description, decimal defaultLimit) : base()
        => (Name, Description, DefaultLimit) = (name, description, defaultLimit);
    #endregion

    #region Factory Methods
    public static OperationResult<Bucket> Create(string name, string description, decimal defaultLimit)
        => Validate(name, description, defaultLimit)
            .IfSuccessThenReturn(() => new Bucket(name, description, defaultLimit));
    #endregion

    #region Methods
    public void Update(string name, string description, decimal defaultLimit)
        => Validate(name, description, defaultLimit)
            .IfSuccess(res => (Name, Description, DefaultLimit) = (name, description, defaultLimit));

    public MonthlyBucket CreateMonthly(short year, short month)
        => new(year, month, this);

    #endregion
}