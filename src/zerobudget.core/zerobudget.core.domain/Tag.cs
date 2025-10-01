using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class Tag : AggregateRoot<int>
{
    #region Properties
    public string Name { get; protected set; } = string.Empty;
    #endregion

    #region Constructors
    protected Tag() : base() { }
    protected Tag(string name) : base()
        => Name = name.ToLowerInvariant();
    #endregion

    #region Factory Methods
    public static OperationResult<Tag> Create(string name)
        => Validate(name)
            .IfSuccessThenReturn(() => new Tag(name));
    #endregion
}
