using ECO;
using Resulz;

namespace zerobudget.core.domain;

public partial class Tag : AggregateRoot<int>
{
    #region Properties
    public string Name { get; protected set; } = string.Empty;
    public string Description { get; protected set; } = string.Empty;
    #endregion

    #region Constructors
    protected Tag() : base() { }
    protected Tag(string name, string description) : base()
    {
        Name = name;
        Description = description;
    }
    #endregion

    #region Factory Methods
    public static OperationResult<Tag> Create(string name, string description)
        => Validate(name, description)
            .IfSuccessThenReturn(() => new Tag(name, description));
    #endregion

    #region Methods
    public OperationResult Update(string name, string description)
        => Validate(name, description)
            .IfSuccess(res => 
            {
                Name = name;
                Description = description;
            });
    #endregion
}
