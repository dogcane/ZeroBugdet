using ECO;
using Resulz;

namespace zerobudget.core.domain;

public sealed partial class Tag : AggregateRoot<int>
{
    #region Properties
    public string Name { get; private set; } = string.Empty;
    #endregion

    #region Constructors
    private Tag() : base() { }
    private Tag(string name) : base()
        => Name = name.ToLowerInvariant();
    #endregion

    #region Factory Methods
    public static OperationResult<Tag> Create(string name)
        => Validate(name)
            .IfSuccessThenReturn(() => new Tag(name));
    #endregion
}
